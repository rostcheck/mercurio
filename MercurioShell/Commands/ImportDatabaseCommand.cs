using System;
using CommandLine.Utility;
using System.Collections.Generic;
using Mercurio.Domain;
using System.IO;

namespace MercurioShell
{
    public class ImportDatabaseCommand : CommandBase, IExecutableMercurioCommand
    {
        private char[] escapeChars = new char[] { '"', '\'' };

        public ImportDatabaseCommand()
        {
            AddRequiredParameter("database-name", "name");
            AddRequiredParameter("path", "path");
            AddAlias("importdb");
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var databaseName = arguments["database-name"];
            var path = arguments["path"];

            var returnList = new List<string>();
            if (context.OpenContainer.ContainsDocument(databaseName))
            {
                var existingDocumentVersion = context.OpenContainer.GetLatestDocumentVersionMetadata(databaseName);
                if (existingDocumentVersion != null && existingDocumentVersion.IsDeleted)
                {
                    returnList.Add(string.Format("Database {0} exists in container {1} but is is deleted.", databaseName, context.OpenContainer.Name));
                    returnList.Add("You can undelete it with undelete-document or permanently delete it");
                    returnList.Add("using delete-document and the hard-delete option");
                    return returnList;
                }
                else
                {
                    return new List<string>() { string.Format("Database {0} already exists in container {1}", databaseName, context.OpenContainer.Name) };
                }
            }

            // Read file and create schema
            bool hasHeader = false;
            var schema = ParseSchema(path, out hasHeader);
            context.OpenContainer.CreateDatabase(databaseName);

            var lines = new List<string>(File.ReadLines(path));
            var records = new List<Record>();
            for (int lineCounter = (hasHeader) ? 1 : 0; lineCounter < lines.Count; lineCounter++)
            {
                var fields = lines[lineCounter].Split(',');
                var atomicElements = new List<IAtomicDataElement>();
                for (int fieldCounter = 0; fieldCounter < schema.Fields.Count; fieldCounter++)
                {
                    string strippedField = fields[fieldCounter].Trim(escapeChars);
                    atomicElements.Add(GetDataElement(strippedField, schema, lineCounter, fieldCounter));

                    records.Add(Record.Create(lineCounter.ToString(), atomicElements));
                }
            }
            context.OpenContainer.AddDatabaseRecords(databaseName, records, context.Environment.GetActiveIdentity());
            context.OpenContainer.AttachDatabaseSchema(databaseName, schema, context.Environment.GetActiveIdentity());

            return returnList;
        }

        private IAtomicDataElement GetDataElement(string field, DatabaseSchema schema, int lineCounter, int fieldCounter)
        {            
            switch (schema.Fields[fieldCounter].ElementType)
            {                
                case DataElementType.DateTime:
                    return new DateTimeDataElement(lineCounter.ToString(), DateTime.Parse(field));                    
                case DataElementType.FloatingPoint:
                    return new FloatingPointDataElement(lineCounter.ToString(), double.Parse(field));
                case DataElementType.Integer:
                    return new IntegerDataElement(lineCounter.ToString(), int.Parse(field));
                default:
                    return new StringDataElement(lineCounter.ToString(), field);
            }
        }

        private DatabaseSchema ParseSchema(string path, out bool hasHeader)
        {            
            // TODO: currently only supports CSV, generalize to other file types
            if (!path.ToLower().Contains(".csv"))
                throw new ArgumentException("Only .csv files are supported for import");

            hasHeader = true;
            var lines = new List<string>(File.ReadLines(path));
            var schema = new DatabaseSchema();
            if (lines.Count == 0)
                throw new ArgumentException("File contains no data");

            // Get header names
            var headerNames = GetHeaderNames(new List<string>(lines[0].Split(',')), out hasHeader);
            // Find the field types
            int startLine = (lines.Count > 2) ? 2 : 1;
            var fields = new List<string>(lines[startLine].Split(','));
            for (int fieldCounter = 0; fieldCounter < fields.Count; fieldCounter++)
            {
                schema.AddField(new Field(headerNames[fieldCounter], GetFieldType(fields[fieldCounter])));
            }
            return schema;
        }

        private List<string> GetHeaderNames(List<string> fields, out bool hasHeader)
        {
            var returnList = new List<string>(fields.Count);
            hasHeader = true;
            foreach (var field in fields)
            {
                if (GetFieldType(field) != DataElementType.String)
                {
                    hasHeader = false;
                    break;
                }
            }
            if (hasHeader)
            {
                foreach (var field in fields)
                    returnList.Add(field);
            }
            else
            {
                for (int counter = 1; counter <= fields.Count; counter++)
                    returnList.Add("Field " + counter);
            }

            return returnList;
        }

        private DataElementType GetFieldType(string field)
        {
            DataElementType returnType = DataElementType.String; // Default
            string strippedField = field.Trim(escapeChars);

            int intVal;
            double doubleVal;
            DateTime dateTimeVal;
            TimeSpan timeSpanVal;

            if (double.TryParse(strippedField, out doubleVal))
                returnType = DataElementType.FloatingPoint;
            else if (int.TryParse(strippedField, out intVal))
                returnType = DataElementType.Integer;
            else if (TimeSpan.TryParse(strippedField, out timeSpanVal))
                returnType = DataElementType.TimeSpan;            
            else if (DateTime.TryParse(strippedField, out dateTimeVal))
                returnType = DataElementType.DateTime;
            return returnType;
        }
    }
}
