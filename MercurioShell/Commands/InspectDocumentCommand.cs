using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class InspectDocumentCommand : CommandBase, IExecutableMercurioCommand
    {
        private const int numberOfLinesToShow = 15;

        public InspectDocumentCommand()
        {
            AddRequiredParameter("document-name", "name");
            AddAlias("inspect");
        }

        protected override ICollection<string> Execute(string command, Arguments arguments, MercurioShellContext context)
        {
            VerifyContainerIsOpen(context);

            var documentName = arguments["document-name"];
            var documentVersion = context.OpenContainer.GetLatestDocumentVersion(documentName);
            if (documentVersion == null)
                return new List<string>() { "Document " + documentName + " not found in container " + context.OpenContainer.Name };
            var linesToShow = documentVersion.DocumentContent.Split(new string[] { System.Environment.NewLine }, numberOfLinesToShow + 1, StringSplitOptions.None).Take(numberOfLinesToShow).ToList();
            if (linesToShow.Count == numberOfLinesToShow)
                linesToShow.Add("...");
            return new List<string>(linesToShow);
        }        
    }
}
