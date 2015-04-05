using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Information about a DocumentVersion
    /// </summary>
    [Serializable]
    public class DocumentVersionMetadata : ISerializable
    {
        private const string IdSerializationName = "Id";
        private const string PriorVersionIdSerializationName = "PriorVersionId";
        private const string DocumentIdSerializationName = "DocumentId";
        private const string CreatedDateTimeSerializationName = "CreatedDateTime";
        private const string CreatorIdSerializationName = "CreatorId";

        public Guid Id { get; private set; }
        public Guid PriorVersionId { get; private set; }
        public Guid DocumentId { get; private set; }
        public DateTimeOffset CreatedDateTime { get; private set; }
        public string CreatorId { get; private set; }

        // Needed for serialization
        protected DocumentVersionMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Id = (Guid)info.GetValue(IdSerializationName, typeof(Guid));
            this.PriorVersionId = (Guid)info.GetValue(PriorVersionIdSerializationName, typeof(Guid));
            this.DocumentId = (Guid)info.GetValue(DocumentIdSerializationName, typeof(Guid));
            this.CreatedDateTime = (DateTimeOffset)info.GetValue(CreatedDateTimeSerializationName, typeof(DateTimeOffset));
            this.CreatorId = info.GetString(CreatorIdSerializationName);
        }

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(IdSerializationName, this.Id);
            info.AddValue(PriorVersionIdSerializationName, this.PriorVersionId);
            info.AddValue(DocumentIdSerializationName, this.DocumentId);
            info.AddValue(CreatedDateTimeSerializationName, this.CreatedDateTime);
            info.AddValue(CreatorIdSerializationName, this.CreatorId);

        }
    }
}
