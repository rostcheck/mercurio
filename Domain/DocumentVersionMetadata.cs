using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Information about a DocumentVersion
    /// </summary>
    public class DocumentVersionMetadata
    {
        public Guid Id { get; private set; }
        public Guid PriorVersionId { get; private set; }
        public Guid DocumentId { get; private set; }
        public DateTimeOffset CreatedDateTime { get; private set; }
        public string CreatorId { get; private set; }

        private DocumentVersionMetadata(Guid documentId, Guid priorVersionId, string creatorId)
        {
            this.DocumentId = documentId;
            this.Id = Guid.NewGuid();
            this.PriorVersionId = priorVersionId;
            this.CreatedDateTime = DateTimeOffset.UtcNow;
            this.CreatorId = creatorId;
        }

        internal static DocumentVersionMetadata Create(Guid documentId, Guid priorVersionId, string creatorId)
        {
            return new DocumentVersionMetadata(documentId, priorVersionId, creatorId);
        }
    }
}
