using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ShowDocumentsCommand : CommandBase, IExecutableMercurioCommand
    {
        public ShowDocumentsCommand()
        {
            AddOptionalParameter("show-all", "show-all");            
            AddAlias("Dir");
            AddAlias("ls");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var returnList = new List<string>() { string.Format("Available documents in container {0} are:", context.OpenContainer.Name) };
            if (arguments.Contains("show-all"))
                returnList.AddRange(context.OpenContainer.AllDocuments);
            else
                returnList.AddRange(context.OpenContainer.Documents);
            return returnList;
        }
    }
}
