using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Utility;
using Mercurio.Domain;

namespace MercurioShell
{
    public class CreateContainerCommand : IExecutableMercurioCommand
    {
        public CreateContainerCommand()
        {
        }

        public string Name
        {
            get
            {
                return "Create-Container";
            }
        }

        public bool RecognizeCommand(string commandName)
        {
            return (commandName.ToLower().Contains("create-container"));
        }

        public void ValidateSyntax(string commandName, Arguments args)        
        {
            if (commandName.ToLower().Trim() != "create-container")
                throw new MercurioShellSyntaxException(string.Format("Invalid command name {0}, expected Create-Container", commandName.ToLower().Trim()));
            if (!args.Contains("container-name"))
                throw new MercurioShellSyntaxException("Argument container-name is required");
            if (!args.Contains("substrate-name"))
                throw new MercurioShellSyntaxException("Argument substrate-name is required");
        }

        public string ShowHelp()
        {
            return "Usage: Create-Container [-container-name <name>] [-substrate-name <name>] <-revision-retention-policy [KeepOne | KeepAll]>";
        }

        public ICollection<string> ExecuteCommand(string command, Arguments arguments, MercurioShellContext context)
        {
            if (context == null || context.Environment == null)
                throw new ArgumentException("Invalid context passed to command");

            var container = context.Environment.CreateContainer(arguments["container-name"], arguments["substrate-name"], GetRetentionPolicy(arguments["revision-retention"]));
            return new List<string> { container.Name };
        }

        private RevisionRetentionPolicyType GetRetentionPolicy(string policyName)
        {
            switch (policyName)
            {
                case "keep-all":
                    return RevisionRetentionPolicyType.KeepAll;
                default:
                    return RevisionRetentionPolicyType.KeepOne;
            }
        }
    }
}
