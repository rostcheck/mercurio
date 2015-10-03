using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class RenameDocumentCommand : CommandBase, IExecutableMercurioCommand
    {
        public RenameDocumentCommand()
        {
            AddRequiredParameter("old-document-name", "name");
            AddRequiredParameter("new-document-name", "name");
            AddAlias("Rename");
            AddAlias("Ren");
            AddAlias("mv");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            context.OpenContainer.RenameDocument(arguments["old-document-name"], arguments["new-document-name"]);

            var returnList = new List<string>() { string.Format("Renamed document from {0} to {1}", arguments["old-document-name"], arguments["new-document-name"]) };
            return returnList;
        }
    }
}
