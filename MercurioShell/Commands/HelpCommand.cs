using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class HelpCommand : CommandBase, IExecutableMercurioCommand
    {
        public HelpCommand()
        {
            AddOptionalParameter("Subject", "name");
        }

        public override string Name 
        { 
            get 
            { 
                return "Help";  
            } 
        }

        public override string RewriteBeforeParsing(string commandString)
        {
            var args = new List<string>(commandString.Split());
            if (args.Count() > 1 && !args[1].ToLower().Contains("subject"))
            {
                var newArgs = new string[args.Count() + 1];
                newArgs[0] = args[0];
                newArgs[1] = "-subject";
                for (int argNumber = 2; argNumber < newArgs.Count(); argNumber++)
                    newArgs[argNumber] = args[argNumber - 1];
                commandString = string.Join(" ", newArgs);
            }
            return commandString;
        }

        public ICollection<string> ExecuteCommand(string commandString, Arguments arguments, MercurioShellContext context)
        {
            if (!string.IsNullOrEmpty(arguments["subject"]))
            {
                var command = context.Commands.Where(s => s.Name.ToLower() == arguments["subject"].ToLower()).FirstOrDefault();
                if (command != null)
                    return new List<string>() { command.ShowHelp() };
                else
                    throw new MercurioShellException(string.Format("Command with name {0} not recognized", arguments["subject"]));
            }
            else
            {
                var result = new List<string>();
                result.Add("Available commands are:");
                foreach (var command in context.Commands)
                {
                    result.Add(command.Name);
                }
                return result;
            }
        }
    }
}
