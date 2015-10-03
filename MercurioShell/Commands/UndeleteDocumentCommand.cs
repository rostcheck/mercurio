using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell.Commands
{
    public class UndeleteDocumentCommand : CommandBase
    {
        public UndeleteDocumentCommand()
        {
            AddRequiredParameter("document-name", "name");
            AddAlias("Undel");
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);
            string documentName = arguments["document-name"];

            try
            {
                context.OpenContainer.UnDeleteDocument(documentName, context.Environment.GetActiveIdentity());
                return new List<string>() { string.Format("Document {0} was undeleted", arguments["document-name"]) };
            }
            catch (Exception ex)
            {
                return new List<string>() { ex.Message };
            }
        }
    }
}
