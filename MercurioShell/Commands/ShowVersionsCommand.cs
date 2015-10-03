using CommandLine.Utility;
using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell.Commands
{
    public class ShowVersionsCommand : CommandBase
    {
        public ShowVersionsCommand()
        {
            AddRequiredParameter("document-name", "name");
            AddAlias("Versions");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var returnList = new List<string>();
            var versions = context.OpenContainer.ListAvailableVersions(arguments["document-name"]);
            if (versions == null)
                returnList.Add("Document " + arguments["document-name"] + " was not found in container " + context.OpenContainer.Name);
            else
            {
                returnList = new List<string>() { string.Format("Available versions for document {0} are:", arguments["document-name"]) };
                returnList.AddRange(versions.Select(s => VersionAsString(s, context)));
            }
            return returnList;
        }

        private string VersionAsString(DocumentVersionMetadata metadata, MercurioShellContext context)
        {
            var creator = context.Environment.GetUserIdentity(metadata.CreatorId);
            var creatorName = (creator == null) ? "" : creator.Name;
            return string.Format("{0} {1} {2} {3}", metadata.CreatedDateTime.ToLocalTime(), creatorName, metadata.Id, metadata.IsDeleted ? "(deleted)" : "");
        }
    }
}
