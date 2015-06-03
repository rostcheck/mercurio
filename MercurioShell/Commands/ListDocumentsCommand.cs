using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class ListDocumentsCommand : CommandBase, IExecutableMercurioCommand
    {
        public override string Name
        {
            get
            {
                return "List-Documents";
            }
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            ValidateContext(context);

            if (context.OpenContainer == null)
                return new List<string>() { string.Format("No container is unlocked") };

            var returnList = new List<string>() { string.Format("Available documents in container {0} are:", context.OpenContainer) };
            returnList.AddRange(context.OpenContainer.Documents);
            return returnList;
        }
    }
}
