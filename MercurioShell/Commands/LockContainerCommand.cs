using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class LockContainerCommand : CommandBase, IExecutableMercurioCommand
    {
        public LockContainerCommand()
        {
            AddOptionalParameter("container-name", "name");
            AddAlias("Close-Container");
            AddAlias("Close");
            AddAlias("Lock");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            var containerName = arguments["container-name"];
            var container = (containerName == null) ? context.OpenContainer : context.Environment.GetContainer(containerName);
            if (container == null)
                return new List<string>() { string.Format("Container named {0} was not found", arguments["container-name"]) };

            context.Environment.LockContainer(container);
            context.OpenContainer = null;
            return new List<string> { string.Format("Locked container {0}", container.Name) };
        }
    }
}
