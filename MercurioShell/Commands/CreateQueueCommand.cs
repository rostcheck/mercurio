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
			AddRequiredParameter("queue-type", new string[] { "LocalDisk", "RemoteAzure", "RemoteAWS" });
			AddRequiredParameter("queue-info",	"info");
			AddAlias("mkq");			
		}


		protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
		{
			var queue = context.Environment.CreateQueue(arguments["queue-name"], arguments["queue-type"], arguments["queue-info"]);
			return new List<string> { string.Format("Created queue {0}", queue.Name) };
		}
	}
}



