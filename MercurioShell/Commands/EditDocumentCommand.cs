using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell.Commands
{
    public class EditDocumentCommand : CommandBase
    {
        public EditDocumentCommand()
        {
            AddRequiredParameter("document-name", "name");
        }

        public override string Name
        {
            get
            {
                return "Edit-Document";
            }
        }

        protected override ICollection<string> Execute(string commandName, Arguments arguments, MercurioShellContext context)
        {
            string documentName = arguments["document-name"];
            if (!documentName.ToLower().Contains(".txt"))
                documentName = string.Format("{0}.txt", documentName);

            bool editing = context.OpenContainer.ContainsDocument(documentName);
            var existingDocumentVersion =  (editing == true) ? context.OpenContainer.GetLatestDocumentVersion(documentName) : null;
            Guid documentId = (existingDocumentVersion != null) ? existingDocumentVersion.DocumentId : Guid.NewGuid();
            string existingDocumentContent = (editing == true) ? existingDocumentVersion.DocumentContent : string.Empty;
            var editedContent = context.Environment.EditDocument(documentId.ToString(), existingDocumentContent);
            if (editing == true)
                context.OpenContainer.ModifyTextDocument(documentName, context.Environment.GetActiveIdentity(), editedContent);
            else
                context.OpenContainer.CreateTextDocument(documentName, context.Environment.GetActiveIdentity(), editedContent);
            return new List<string>() { string.Format("{0} document {1}", (editing == true) ? "Edited" : "Created", documentName) };
        }
    }
}
