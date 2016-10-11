using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Utility;
using Mercurio.Domain;

namespace MercurioShell
{
	public class CreateQueueCommand : CommandBase, IExecutableMercurioCommand
	{
		public CreateQueueCommand()
		{
			//TODO: finish implementation
			AddRequiredParameter("queue-name", "name");
			AddRequiredParameter("queue-type", new string[] { "Local", "Remote" });
			//AddOptionalParameter("revision-retention-policy", new string[] { "KeepOne", "KeepAll" });
			AddAlias("mkq");			
		}


		protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
		{
			var queue = context.Environment.CreateQueue(arguments["queue-name"], arguments["queue-type"]);
			return new List<string> { string.Format("Created queue {0}", queue.Name) };
		}
	}
}



