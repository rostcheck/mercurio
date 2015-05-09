using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class CommandBase
    {
        private List<CommandArgument> _arguments;
        public CommandBase()
        {
            _arguments = new List<CommandArgument>();
        }

        public virtual string Name
        {
            get
            {
                return "";
            }
        }

        public virtual string ShowHelp()
        {
            var sb = new StringBuilder();
            foreach (var argument in _arguments.Where(s => s.Required == true))
                sb.Append(string.Format("-{0} {1} ", argument.Name, FormatArgumentHelp(argument)));

            foreach (var argument in _arguments.Where(s => s.Required == false))
                sb.Append(string.Format("[-{0} {1}] ", argument.Name, FormatArgumentHelp(argument)));

            return string.Format("Usage: {0} {1}", Name, sb.ToString());
        }

        private string FormatArgumentHelp(CommandArgument argument)
        {
            if (argument.AllowedValues != null)
                return string.Format("<{0}>", string.Join(" | ", argument.AllowedValues));
            else
                return string.Format("<{0}>", argument.ArgumentNameForHelp);
        }

        public virtual string RewriteBeforeParsing(string commandString)
        {
            return commandString;
        }

        public virtual bool RecognizeCommand(string commandName)
        {
            return (commandName.ToLower().Contains(Name.ToLower()));
        }

        public virtual void ValidateSyntax(string commandName, Arguments args)
        {
            if (commandName.ToLower().Trim() != Name.ToLower())
                throw new MercurioShellSyntaxException(string.Format("Invalid command name {0}, expected {1}", commandName.ToLower().Trim(), Name));

            foreach (var argument in _arguments.Where(s => s.Required == true))
            {
                if (!args.Contains(this.Name))
                    throw new MercurioShellSyntaxException(string.Format("Argument {0} is required", argument.Name));
            }
        }

        internal void ValidateContext(MercurioShellContext context)
        {
            if (context == null || context.Environment == null)
                throw new ArgumentException("Invalid context passed to command");
        }

        internal void AddOptionalParameter(string name, string argumentNameForHelp)
        {
            _arguments.Add(new CommandArgument(name, false, argumentNameForHelp));
        }

        internal void AddOptionalParameter(string name, string[] allowedValues)
        {
            _arguments.Add(new CommandArgument(name, false, "", allowedValues));
        }

        internal void AddRequiredParameter(string name, string argumentNameForHelp)
        {
            _arguments.Add(new CommandArgument(name, true, argumentNameForHelp));
        }

        internal void AddRequiredParameter(string name, string[] allowedValues)
        {
            _arguments.Add(new CommandArgument(name, true, "", allowedValues));
        }
    }
}
