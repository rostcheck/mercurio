using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ImportDocumentCommand : CommandBase, IExecutableMercurioCommand
    {
        public ImportDocumentCommand()
        {
            AddRequiredParameter("document-path", "path");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            ValidateContext(context);

            if (context.OpenContainer == null)
                return new List<string>() { string.Format("No container is unlocked") };

            var documentPath = arguments["document-path"];
            var documentName = Path.GetFileName(documentPath);
            var documentData = File.ReadAllText(documentPath);
            var documentVersion = context.OpenContainer.CreateTextDocument(documentName, context.Environment.GetActiveIdentity(), documentData);
            var returnList = new List<string>() { string.Format("Imported document {0} into container {1}", documentName, context.OpenContainer.Name) };
            returnList.AddRange(context.OpenContainer.Documents);
            return returnList;
        }
    }
}
