using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ShowContainerCommand : IExecutableMercurioCommand
    {
        public ShowContainerCommand()
        {
        }

        public bool RecognizeCommand(string commandName)
        {
            return (commandName.ToLower().Contains("show-containers"));
        }

        public void ValidateSyntax(string commandName, Arguments args)
        {
            // Valid synax is show-containers w/ no args
            if (commandName.ToLower().Trim() != "show-containers")
                throw new MercurioShellSyntaxException(string.Format("Invalid command name {0}, expected Show-Containers", commandName.ToLower().Trim()));
        }

        public string ShowHelp()
        {
            return "Usage: Show-Containers";
        }

        public ICollection<string> ExecuteCommand(string command, Arguments args, MercurioShellContext context)
        {
            if (context == null || context.Environment == null)
                throw new ArgumentException("Invalid context passed to command");

            return context.Environment.GetContainers().Select(s => s.Name).ToList();
        }
    }
}
