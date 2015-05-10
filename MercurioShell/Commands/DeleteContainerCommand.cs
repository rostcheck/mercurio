using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class DeleteContainerCommand : CommandBase
    {
        public DeleteContainerCommand()
        {
            AddRequiredParameter("container-name", "name");
        }

        public override string Name
        {
            get
            {
                return "Delete-Container";
            }
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            if (context.ConfirmAction("WARNING: Deleting a container will delete all its contents forever. Are you sure you want to do this?", context.Environment))
            {
                context.Environment.DeleteContainer(arguments["container-name"]);
                return new List<string>() { string.Format("Container {0} was deleted", arguments["container-name"]) };
            }
            else
                return null;
        }
    }
}
