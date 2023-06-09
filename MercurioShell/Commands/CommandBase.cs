﻿using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public abstract class CommandBase : IExecutableMercurioCommand
    {
        private List<CommandArgument> _arguments;
        private string _commandName;
        public List<string> _aliases;

        public CommandBase()
        {
            _arguments = new List<CommandArgument>();
            _commandName = ParseCommandName();
        }

        private string ParseCommandName()
        {
            var name = this.GetType().Name.Replace("Command", "");
            bool inLowerCase = false;
            var sb = new StringBuilder();
            for (int counter = 0; counter < name.Length; counter++)
            {
                // If we're going from lower to upper case, add a hyphen
                if (char.IsUpper(name[counter]) && inLowerCase == true)
                {
                    sb.Append("-");
                }
                inLowerCase = char.IsLower(name[counter]);
                sb.Append(name[counter]);
            }
            return sb.ToString();
        }

        public string Name
        {
            get
            {
                return _commandName;
            }
        }

        public virtual string ShowHelp()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Usage: {0} ", Name);
            foreach (var argument in _arguments.Where(s => s.Required == true))
                sb.Append(string.Format("-{0} {1} ", argument.Name, FormatArgumentHelp(argument)));

            foreach (var argument in _arguments.Where(s => s.Required == false))
                sb.Append(FormatNameAndHelp(argument));

            if (_aliases != null)
            {
                sb.AppendLine();
                sb.AppendFormat("Aliases: ");
                for (int aliasNumber = 0; aliasNumber < _aliases.Count; aliasNumber++)
                {
                    sb.Append(_aliases[aliasNumber]);
                    if (aliasNumber < _aliases.Count - 1)
                        sb.Append(", ");
                }
            }
            return sb.ToString();
        }

        private string FormatNameAndHelp(CommandArgument argument)
        {
            if (argument.AllowedValues == null)
                return string.Format("[-{0}] ", argument.Name);
            else
                return string.Format("[-{0} {1}] ", argument.Name, FormatArgumentHelp(argument));
        }

        private string FormatArgumentHelp(CommandArgument argument)
        {
            if (argument.AllowedValues == null)
                return string.Format("<{0}>", argument.ArgumentNameForHelp);
            else
                return string.Format("<{0}>", string.Join(" | ", argument.AllowedValues));
        }

        public virtual string RewriteBeforeParsing(string commandString)
        {
            // If only one argument is required, rewrite to implicitly put the argument name in so typing it isn't required
            var parts = commandString.Split();
            var requiredArgs = _arguments.Where(s => s.Required == true).ToList();
			if (parts.Length == 2 && requiredArgs.Count == 1 && !parts[1].Contains("="))
                return string.Format("{0} -{1}={2}", parts[0], requiredArgs[0].Name, parts[1]);
            else
                return commandString;
        }

        public virtual bool RecognizeCommand(string commandName)
        {
            if (commandName.ToLower().Trim() == Name.ToLower())
                return true;
            else
            {
                if (_aliases == null)
                    return false;
                else
                {
                    foreach (var alias in _aliases)
                    {
                        if (commandName.ToLower() == alias.ToLower())
                            return true;
                    }
                    return false;
                }
            }
        }

		// Return the command (or alias) name if the partialCommandName could be part of this command
		public virtual string RecognizePartialCommand(string partialCommand)
		{
			string partialCommandName = partialCommand.Trim().ToLower();
			if (Name.ToLower().StartsWith(partialCommandName))
				return Name;
			else
			{
				if (_aliases == null)
					return null;
				else
				{
					foreach (var alias in _aliases)
					{
						if (alias.ToLower().StartsWith(partialCommandName))
							return alias;
					}
					return null;
				}
			}
		}

		// Return a list of argument names that could complete the partialArgument supplied
		public virtual ICollection<string> RecognizePartialArgument(string partialArgument)
		{
			var possibleArguments = new List<string>();
			string partialArgumentName = partialArgument.Trim().ToLower();
			if (partialArgumentName.StartsWith("-"))
				partialArgumentName = partialArgumentName.Substring(1);


			foreach (var argumentName in _arguments.Select(s => s.Name))
			{
				if (argumentName.ToLower().StartsWith(partialArgumentName))
					possibleArguments.Add("-" + argumentName);
			}
			return possibleArguments;
		}

		// Return a list of arguments that the user has not already filled out on the command line
//		public virtual ICollection<CommandArgument> AvailableArguments(string commandString)
//		{
//			
//			var availableArguments = new List<string>();
//			commandString = commandString.Trim().ToLower();
//
//			foreach (var argumentName in _arguments.Select(s => s.Name))
//			{
//				if (argumentName.ToLower().StartsWith(partialArgumentName))
//					availableArguments.Add(argumentName);
//			}
//			return (availableArguments.Count == 0) ? null : availableArguments;
//		}

        public virtual void ValidateSyntax(string commandName, Arguments args)
        {
            if (!RecognizeCommand(commandName))
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Invalid command name {0}, expected {1}", commandName.ToLower().Trim(), Name);
                if (_aliases != null)
                {
                    sb.Append(" or aliases ");
                    sb.Append(string.Join(", ", _aliases));
                }
                throw new MercurioShellSyntaxException(sb.ToString());
            }                

            foreach (var argument in _arguments.Where(s => s.Required == true))
            {
                if (!args.Contains(argument.Name))
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

        internal void AddAlias(string alias)
        {
            if (_aliases == null)
                _aliases = new List<string>();

            _aliases.Add(alias);
        }

        protected abstract ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context);

        public ICollection<string> ExecuteCommand(string commandName, Arguments arguments, MercurioShellContext context)
        {
            ValidateContext(context);
            ValidateSyntax(commandName, arguments);
			var commandsWithDefaultArguments = _arguments.Where(s => s.Required == false && s.AllowedValues != null && s.AllowedValues.Count() > 0);
			// First allowed value is the default for optional arguments
			arguments.SetOptionalArguments(commandsWithDefaultArguments.ToDictionary(s => s.Name, e => e.AllowedValues[0]));
            return Execute(commandName, arguments, context);
        }

        protected void VerifyContainerIsOpen(MercurioShellContext context)
        {
            if (context == null || context.OpenContainer == null)
                throw new Exception("No container is unlocked.");
        }			 
    }
}
