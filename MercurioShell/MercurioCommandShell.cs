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

        public MercurioCommandShell(IMercurioEnvironment environment)
        {
            _commands = new List<IExecutableMercurioCommand>();
            InstallBasicCommands();
            _context = new MercurioShellContext() { Environment = environment };
        }

        public ICollection<string> ExecuteCommand(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
                return null;

            var args = commandString.Split();
            var commandName = args[0];
            var arguments = new Arguments(args);
            var command = RecognizeCommand(commandName);
            if (command == null)
            {
                Console.WriteLine("Command not recognized");
                return null;
            }

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
            InstallCommand(new CreateContainerCommand());
            InstallCommand(new ShowContainerCommand());
        }

        public void InstallCommand(IExecutableMercurioCommand command)
        {
            _commands.Add(command);
        }

        private IExecutableMercurioCommand RecognizeCommand(string commandName)
        {
            foreach (var command in _commands)
                if (command.RecognizeCommand(commandName))
                    return command;
            return null;
        }

        internal void ShowHelp()
        {
            Console.WriteLine("Available commands are:");
            foreach (var command in _commands)
            {
                Console.WriteLine(command.Name);
            }
        }
    }
}
