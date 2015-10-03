using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class UnlockContainerCommand : CommandBase, IExecutableMercurioCommand
    {
        public UnlockContainerCommand()
        {
            AddRequiredParameter("container-name", "name");
            AddAlias("Open-Container");
            AddAlias("Open");
            AddAlias("Unlock");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            var returnStrings = new List<string>();

            if (context.OpenContainer != null) {
                var lockCommand = new LockContainerCommand();
                var lockArguments = new Arguments(string.Format("-container-name {0}", context.OpenContainer.Name).Split());
                returnStrings = lockCommand.ExecuteCommand(lockCommand.Name, lockArguments, context).ToList();
            }
                
            var container = context.Environment.GetContainer(arguments["container-name"]);
            if (container == null)
            {
                returnStrings.Add(string.Format("Container named {0} was not found", arguments["container-name"]));
                return returnStrings;
            }

            context.Environment.UnlockContainer(container);
            context.OpenContainer = container;
            returnStrings.Add(string.Format("Unlocked container {0}", container.Name));
            return returnStrings;
        }
    }
}
