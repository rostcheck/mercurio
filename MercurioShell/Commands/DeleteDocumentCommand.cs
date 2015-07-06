using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class DeleteDocumentCommand : CommandBase
    {
        public DeleteDocumentCommand()
        {
            AddRequiredParameter("document-name", "name");
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            if (context.ConfirmAction("WARNING: Deleting a document will delete all its contents forever. Are you sure you want to do this?", context.Environment))
            {
                context.OpenContainer.DeleteDocumentSoft(arguments["container-name"], context.Environment.GetActiveIdentity());
                return new List<string>() { string.Format("Document {0} was deleted", arguments["document-name"]) };
            }
            else
                return new List<string>() { "Passphrase not correct - document not deleted" };
        }
    }
}
