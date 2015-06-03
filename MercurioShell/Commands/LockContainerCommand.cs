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
            AddRequiredParameter("container-name", "name");
        }

        public override string Name
        {
            get
            {
                return "Lock-Container";
            }
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            ValidateContext(context);

            var container = context.Environment.GetContainer(arguments["container-name"]);
            if (container == null)
                return new List<string>() { string.Format("Container named {0} was not found", arguments["container-name"]) };

            context.Environment.LockContainer(container);
            context.OpenContainer = null;
            return new List<string> { string.Format("Locked container {0}", container.Name) };
        }
    }
}
