using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A version of a Document. Versions are immutable; changing a Document produces a new DocumentVersion.
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

        public bool IsDeleted
        {
            get
            {
                return Metadata.IsDeleted;
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

        protected DocumentVersion(Guid documentId, Guid priorVersionId, long priorVersionCreatedTimeTicks, string creatorId, bool isDeleted = false)
        {
            this.Metadata = DocumentVersionMetadata.Create(documentId, priorVersionId, priorVersionCreatedTimeTicks, creatorId, isDeleted);
        }

        protected DocumentVersion(DocumentVersion documentVersion, string unencryptedDocumentContent)
        {
            this.Metadata = documentVersion.Metadata;
            this.DocumentContent = unencryptedDocumentContent;
        }

        //private DocumentVersion(Guid priorRevisionId, string creatorId, List<AtomicDataElementChange> changes)
        //    : this(priorRevisionId, creatorId)
        //{
        //    this._changeList = new List<AtomicDataElementChange>(changes);
        //}

        protected DocumentVersion(Guid documentId, Guid priorVersionId, long priorVersionCreatedTimeTicks, string creatorId, string documentContent, bool isDeleted)
            : this(documentId, priorVersionId, priorVersionCreatedTimeTicks, creatorId, isDeleted)
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

        public static DocumentVersion Create(Guid documentId, Guid priorVersionId, long priorVersionCreatedTimeTicks, string creatorId, string documentContent, bool isDeleted)
        {
            return new DocumentVersion(documentId, priorVersionId, priorVersionCreatedTimeTicks, creatorId, documentContent, isDeleted);
        }

        public static DocumentVersion CreateWithUnencryptedContent(DocumentVersion documentVersion, string unencryptedDocumentContent)
        {
            return new DocumentVersion(documentVersion, unencryptedDocumentContent);
        }

        public static DocumentVersion CreateDeleted(Guid documentId, Guid priorVersionId, long priorVersionCreatedTimeTicks, string creatorId)
        {
            return new DocumentVersion(documentId, priorVersionId, priorVersionCreatedTimeTicks, creatorId, true);
        }
    }
}
