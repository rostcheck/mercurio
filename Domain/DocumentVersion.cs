using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A version of a Document. Versions are immutable; changing a Document produces a new Version.
    /// </summary>
    public class DocumentVersion
    {
        public DocumentVersionMetadata Metadata { get; private set; }
        //private List<AtomicDataElementChange> _changeList;
        public string DocumentContent { get; private set; }

        public Guid DocumentId 
        { 
            get
            {
                return Metadata.DocumentId;
            }
        }

        public Guid Id
        {
            get
            {
                return Metadata.Id;
            }
        }

        public Guid PriorVersionId 
        {
            get
            {
                return Metadata.PriorVersionId;
            }
        }

        public DateTimeOffset CreatedDateTime
        { 
            get
            {
                return Metadata.CreatedDateTime;
            }
        }

        public string CreatorId 
        { 
            get
            {
                return Metadata.CreatorId;
            }
        }

        protected DocumentVersion(Guid documentId, Guid priorVersionId, string creatorId)
        {
            this.Metadata = DocumentVersionMetadata.Create(documentId, priorVersionId, creatorId);
        }

        //private DocumentVersion(Guid priorRevisionId, string creatorId, List<AtomicDataElementChange> changes)
        //    : this(priorRevisionId, creatorId)
        //{
        //    this._changeList = new List<AtomicDataElementChange>(changes);
        //}

        protected DocumentVersion(Guid documentId, Guid priorVersionId, string creatorId, string documentContent)
            : this(documentId, priorVersionId, creatorId)
        {
            this.DocumentContent = documentContent;
        }

        //public List<AtomicDataElementChange> GetChanges()
        //{
        //    return new List<AtomicDataElementChange>(_changeList);
        //}

        //internal static DocumentVersion Create(Guid priorRevisionGuid, string revisorIdentityUniqueId, List<AtomicDataElementChange> changes)
        //{
        //    return new DocumentVersion(priorRevisionGuid, revisorIdentityUniqueId, changes);
        //}

        internal static DocumentVersion Create(Guid documentId, Guid priorVersionId, string creatorId, string documentContent)
        {
            return new DocumentVersion(documentId, priorVersionId, creatorId, documentContent);
        }
    }
}
