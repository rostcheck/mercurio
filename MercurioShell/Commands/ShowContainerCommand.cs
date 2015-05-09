using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ShowContainerCommand : CommandBase, IExecutableMercurioCommand
    {
        public override string Name
        {
            get
            {
                return "Show-Containers";
            }
        }

        public ICollection<string> ExecuteCommand(string command, Arguments args, MercurioShellContext context)
        {
            ValidateContext(context);

            return context.Environment.GetContainers().Select(s => s.Name).ToList();
        }
    }
}
