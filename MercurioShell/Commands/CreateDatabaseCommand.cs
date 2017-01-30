using System;
using CommandLine.Utility;
using System.Collections.Generic;

namespace MercurioShell
{
    public class CreateDatabaseCommand : CommandBase, IExecutableMercurioCommand
    {
        public CreateDatabaseCommand()
        {
            AddRequiredParameter("database-name", "name");
            AddOptionalParameter("no-schema", "no-schema");
            AddAlias("mkdb");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var databaseName = arguments["database-name"];
            var schemaName = databaseName + "-schema";

            var returnList = new List<string>();
            if (context.OpenContainer.ContainsDocument(databaseName))
            {
                var existingDocumentVersion = context.OpenContainer.GetLatestDocumentVersion(databaseName);
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
                                                           
            if (!arguments.Contains("no-schema"))
            {
                bool editing = context.OpenContainer.ContainsDocument(schemaName);
                var existingDocumentVersion = (editing == true) ? context.OpenContainer.GetLatestDocumentVersion(schemaName) : null;
                if (existingDocumentVersion != null && existingDocumentVersion.IsDeleted)
                    context.OpenContainer.UnDeleteDocument(schemaName, context.Environment.GetActiveIdentity());
                Guid documentId = (existingDocumentVersion != null) ? existingDocumentVersion.DocumentId : Guid.NewGuid();
                string existingDocumentContent = (editing == true) ? existingDocumentVersion.DocumentContent : string.Empty;
                var editedContent = context.Environment.EditDocument(documentId.ToString(), existingDocumentContent);
                if (editing == true)
                    context.OpenContainer.ModifyTextDocument(schemaName, context.Environment.GetActiveIdentity(), editedContent);
                else
                    context.OpenContainer.CreateTextDocument(schemaName, context.Environment.GetActiveIdentity(), editedContent);
            }
            context.OpenContainer.CreateTextDocument(databaseName, context.Environment.GetActiveIdentity(), "");
            returnList.Add(string.Format("Created database {0} in container {1}", databaseName, context.OpenContainer.Name));
            returnList.AddRange(context.OpenContainer.Documents);
            return returnList;
        }
    }
}
