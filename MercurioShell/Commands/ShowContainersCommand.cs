using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ShowContainersCommand : CommandBase
    {
        public ShowContainersCommand()
        {
            AddAlias("Show");
        }

        protected override ICollection<string> Execute(string command, Arguments args, MercurioShellContext context)
        {
            return context.Environment.GetContainers().Select(s => s.Name).ToList();
        }
    }
}
