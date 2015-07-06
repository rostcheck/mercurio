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
        private const string IsDeletedSerializationName = "IsDeleted";

        public Guid Id { get; private set; }
        public Guid PriorVersionId { get; private set; }
        public Guid DocumentId { get; private set; }
        public DateTimeOffset CreatedDateTime { get; private set; }
        public string CreatorId { get; private set; }
        public bool IsDeleted { get; private set; }

        // Needed for serialization
        protected DocumentVersionMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Id = (Guid)info.GetValue(IdSerializationName, typeof(Guid));
            this.PriorVersionId = (Guid)info.GetValue(PriorVersionIdSerializationName, typeof(Guid));
            this.DocumentId = (Guid)info.GetValue(DocumentIdSerializationName, typeof(Guid));
            this.CreatedDateTime = (DateTimeOffset)info.GetValue(CreatedDateTimeSerializationName, typeof(DateTimeOffset));
            this.CreatorId = info.GetString(CreatorIdSerializationName);
            this.IsDeleted = info.GetBoolean(IsDeletedSerializationName);
        }

        private DocumentVersionMetadata(Guid documentId, Guid priorVersionId, long priorVersionCreatedTimeTicks, string creatorId, bool isDeleted)
        {
            this.DocumentId = documentId;
            this.Id = Guid.NewGuid();
            this.PriorVersionId = priorVersionId;
            this.CreatedDateTime = DateTimeOffset.UtcNow;
            if (priorVersionCreatedTimeTicks != 0 && this.CreatedDateTime.UtcTicks == priorVersionCreatedTimeTicks)
                this.CreatedDateTime = this.CreatedDateTime.AddTicks(1000); // Guard against creation in same clock tick
            this.CreatorId = creatorId;
            this.IsDeleted = isDeleted;
        }

        internal static DocumentVersionMetadata Create(Guid documentId, Guid priorVersionId, long priorVersionCreatedTimeTicks, string creatorId, bool isDeleted = false)
        {
            return new DocumentVersionMetadata(documentId, priorVersionId, priorVersionCreatedTimeTicks, creatorId, isDeleted);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(IdSerializationName, this.Id);
            info.AddValue(PriorVersionIdSerializationName, this.PriorVersionId);
            info.AddValue(DocumentIdSerializationName, this.DocumentId);
            info.AddValue(CreatedDateTimeSerializationName, this.CreatedDateTime);
            info.AddValue(CreatorIdSerializationName, this.CreatorId);
            info.AddValue(IsDeletedSerializationName, this.IsDeleted);
        }
    }
}
