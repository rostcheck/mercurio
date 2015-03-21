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

        public string DocumentName { get; private set; }
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

        protected DocumentVersion(Guid priorVersionId, string creatorId)
        {
            this.Metadata = DocumentVersionMetadata.Create(priorVersionId, creatorId);
        }

        //private DocumentVersion(Guid priorRevisionId, string creatorId, List<AtomicDataElementChange> changes)
        //    : this(priorRevisionId, creatorId)
        //{
        //    this._changeList = new List<AtomicDataElementChange>(changes);
        //}

        protected DocumentVersion(Guid priorVersionId, string creatorId, string documentContent)
            : this(priorVersionId, creatorId)
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

        internal static DocumentVersion Create(Guid priorVersionId, string creatorId, string documentContent)
        {
            return new DocumentVersion(priorVersionId, creatorId, documentContent);
        }
    }
}
