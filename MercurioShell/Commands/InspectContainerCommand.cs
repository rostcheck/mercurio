using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Utility;
using Mercurio.Domain;

namespace MercurioShell
{
	public class InspectContainerCommand : CommandBase, IExecutableMercurioCommand
	{
		public InspectContainerCommand()
		{
			AddRequiredParameter("container-name", "name");
		}

		protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
		{
			var container = context.Environment.GetContainer(arguments["container-name"]);
			var containerInfo = new List<string>();
			containerInfo.Add(string.Format("Container name: {0}", container.Name));
			containerInfo.Add(string.Format("Locked: {0}", container.IsLocked ? "yes" : "no"));
			containerInfo.Add(string.Format("Crypto manager type: {0}", container.CryptoManagerType));
			if (!container.IsLocked)
			{
				containerInfo.Add(string.Format("Revision retention policy: {0}", container.RevisionRetentionPolicyType));
			}

			return containerInfo;
		}

		private RevisionRetentionPolicyType GetRetentionPolicy(string policyName)
		{
			if (policyName == null)
				return RevisionRetentionPolicyType.KeepOne;

			switch (policyName.ToLower())
			{
				case "keepall":
					return RevisionRetentionPolicyType.KeepAll;
				default:
					return RevisionRetentionPolicyType.KeepOne;
			}
		}
	}
}
