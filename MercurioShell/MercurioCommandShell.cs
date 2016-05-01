using CommandLine.Utility;
using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    /// <summary>
    /// Interprets a text command and dispatches appropriate behavior
    /// </summary>
    public class MercurioCommandShell
    {
        List<IExecutableMercurioCommand> _commands;
        MercurioShellContext _context;

        protected MercurioCommandShell(IMercurioEnvironment environment, Func<string, IMercurioEnvironment, bool> confirmAction)
        {
            _commands = new List<IExecutableMercurioCommand>();
            InstallBasicCommands();
            _context = new MercurioShellContext() { Environment = environment, Commands = _commands, ConfirmAction = confirmAction };            
        }

        public static MercurioCommandShell Create(IMercurioEnvironment environment, Func<string, IMercurioEnvironment, bool> confirmAction)
        {
            if (environment == null)
                throw new ArgumentNullException("Must supply a valid Mercurio environment");

            if (confirmAction == null)
                throw new ArgumentNullException("Must supply a valid callback to confirm actions");

            return new MercurioCommandShell(environment, confirmAction);
        }

        public ICollection<string> ExecuteCommand(string commandString)
        {
            commandString = commandString.Trim();
            if (string.IsNullOrEmpty(commandString))
                return null;

            var args = commandString.Split();
            var commandName = args[0];
            var command = RecognizeCommand(commandName);
            if (command == null)
            {
                Console.WriteLine("Command not recognized");
                return null;
            }

            var arguments = new Arguments(command.RewriteBeforeParsing(commandString).Split());

            command.ValidateSyntax(commandName, arguments);
            return command.ExecuteCommand(commandName, arguments, _context);
        }

        public void InstallCommand(string dllPath)
        {
            // Open the DLL, use reflection to find classes derived from PSCustomSnapIn, add them to cmdlet dispatch list
            throw new NotImplementedException();
        }

        public void InstallBasicCommands()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(MercurioCommandShell));
            var commandTypes = assembly.ExportedTypes.Where(s => s.Name.EndsWith("Command") && s.IsClass);

            foreach (var commandType in commandTypes)
            {
                InstallCommand(Activator.CreateInstance(commandType) as IExecutableMercurioCommand);
            }
        }

        public void InstallCommand(IExecutableMercurioCommand command)
        {
            _commands.Add(command);
        }

        private IExecutableMercurioCommand RecognizeCommand(string commandName)
        {
            foreach (var command in _commands)
            {
                if (command.RecognizeCommand(commandName))
                    return command;
            }
            return null;
        }

		public string ResolveCommand(string originalCommandString)
		{
			var commandString = originalCommandString.Trim();
			if (string.IsNullOrEmpty(commandString))
				return commandString;

			var args = commandString.Split();
			var commandName = args[0];
			var completions = new List<string>();

			if (args.Length == 1)
			{
				// Command tab completion
				foreach (var command in _commands)
				{
					string recognizedCommand = command.RecognizePartialCommand(commandName);
					if (recognizedCommand != null)
						completions.Add(recognizedCommand);
				}								
			}
			else
			{
				// Handle argument tab completion
				var command = RecognizeCommand(commandName);
				if (command != null)
					completions = new List<string>(command.RecognizePartialArgument(args[args.Count() - 1]));				
			}
			if (completions.Count == 0)
				return originalCommandString;
			else
				return CompleteCommand(commandString, CommonPrefixOf(completions));
		}
			
		private static string CompleteCommand(string commandString, string completedArgument)
		{
			var args = commandString.Split();
			if (args.Count() == 0) 
				return "";
			else
			{
				var lastArgument = args[args.Count() - 1];
				return commandString.Substring(0, commandString.Length - lastArgument.Length) + completedArgument;
			}
		}

		private static string CommonPrefixOf(IEnumerable<string> strings)
		{
			if (strings.Count() == 1)
				return strings.First();
			
			var commonPrefix = strings.FirstOrDefault() ?? "";
			var commonPrefixLower = commonPrefix.ToLower();

			foreach(var testString in strings)
			{
				var s = testString.ToLower();
				var potentialMatchLength = Math.Min(s.Length, commonPrefix.Length);

				if (potentialMatchLength < commonPrefix.Length)
					commonPrefix = commonPrefix.Substring(0, potentialMatchLength);

				for(var i = 0; i < potentialMatchLength; i++)
				{
					if (s[i] != commonPrefixLower[i])
					{
						commonPrefix = commonPrefix.Substring(0, i);
						break;
					}
				}
			}

			return commonPrefix;
		}
    }
}
