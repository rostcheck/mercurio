using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ShowSubstratesCommand : CommandBase, IExecutableMercurioCommand
    {
        public string Name
        {
            get
            {
                return "Show-Substrates";
            }
        }

        public ShowSubstratesCommand()
        {
        }

        public bool RecognizeCommand(string commandName)
        {
            return (commandName.ToLower().Contains("show-substrates"));
        }

        public void ValidateSyntax(string commandName, Arguments args)
        {
            // Valid synax is show-substrates w/ no args
            if (commandName.ToLower().Trim() != "show-substrates")
                throw new MercurioShellSyntaxException(string.Format("Invalid command name {0}, expected Show-Substrates", commandName.ToLower().Trim()));
        }

        public string ShowHelp()
        {
            return "Usage: Show-Substrates";
        }

        public ICollection<string> ExecuteCommand(string command, Arguments args, MercurioShellContext context)
        {
            if (context == null || context.Environment == null)
                throw new ArgumentException("Invalid context passed to command");

            return context.Environment.GetAvailableStorageSubstrateNames();
        }
    }
}
