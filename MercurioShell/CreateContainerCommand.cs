using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Utility;
using Mercurio.Domain;

namespace MercurioShell
{
    public class CreateContainerCommand : CommandBase, IExecutableMercurioCommand
    {
        public CreateContainerCommand()
        {
            AddRequiredParameter("container-name", "name");
            AddRequiredParameter("substrate-name", "name");
            AddOptionalParameter("revision-retention-policy", new string[] { "KeepOne", "KeepAll" });
        }

        public override string Name
        {
            get
            {
                return "Create-Container";
            }
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            ValidateContext(context);

            var container = context.Environment.CreateContainer(arguments["container-name"], arguments["substrate-name"], GetRetentionPolicy(arguments["revision-retention"]));
            return new List<string> { container.Name };
        }

        private RevisionRetentionPolicyType GetRetentionPolicy(string policyName)
        {
            switch (policyName)
            {
                case "keepall":
                    return RevisionRetentionPolicyType.KeepAll;
                default:
                    return RevisionRetentionPolicyType.KeepOne;
            }
        }
    }
}
