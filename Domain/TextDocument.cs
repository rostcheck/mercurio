using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents a Document configured to hold a single (potentially large) block of text
    /// </summary>
    public class TextDocument : IDocument
    {
        private IRevisionRetentionPolicy _revisionRetentionPolicy;
        private List<Revision> _revisions;
        public List<Revision> Revisions 
        { 
            get
            {
                return new List<Revision>(_revisions);
            }             
            private set
            {
                _revisions = value;
            }
        }

        public string Content 
        { 
            get
            {
                return this.Revisions.Last().DocumentContent;
            }
        }

        public string Name { get; private set; }

        private TextDocument(string documentName, IRevisionRetentionPolicy retentionPolicy, Identity creatorIdentity, string initialData)
        {
            this.Name = documentName;
            this._revisionRetentionPolicy = retentionPolicy;
            this.Revisions = new List<Revision>();
            SetContent(initialData, creatorIdentity);
        }

        internal static TextDocument Create(string documentName, IRevisionRetentionPolicy retentionPolicy, Identity creatorIdentity, string initialData)
        {
            return new TextDocument(documentName, retentionPolicy, creatorIdentity, initialData);
        }

        public void SetContent(string content, Identity revisoridentity)
        {
            Guid priorDocumentVersion = (this.Revisions != null && this.Revisions.Count > 0) ? this.Revisions.Last().Id : Guid.Empty;
            this._revisions.Add(Revision.Create(priorDocumentVersion, revisoridentity.UniqueIdentifier, content));
            var revisionsToDelete = _revisionRetentionPolicy.RevisionsToDelete(_revisions);
            foreach (var revision in revisionsToDelete)
            {
                _revisions.Remove(revision);
            }
        }
    }
}
