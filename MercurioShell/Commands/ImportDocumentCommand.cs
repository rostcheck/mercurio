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
            AddAlias("import");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var documentPath = arguments["document-path"];
            var documentName = Path.GetFileName(documentPath);
            var documentData = File.ReadAllText(documentPath);
            context.OpenContainer.CreateTextDocument(documentName, context.Environment.GetActiveIdentity(), documentData);
            var returnList = new List<string>() { string.Format("Imported document {0} into container {1}", documentName, context.OpenContainer.Name) };
            returnList.AddRange(context.OpenContainer.Documents);
            return returnList;
        }
    }
}
