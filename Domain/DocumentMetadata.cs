using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Information about a Document
    /// </summary>
    [Serializable]
    public class DocumentMetadata : ISerializable
    {
        private const string IdSerializationName = "Id";
        private const string NameSerializationName = "Name";
        private const string DocumentTypeSerializationName = "DataType";


        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string DocumentType { get; private set; }

        private DocumentMetadata(string documentName, string documentType)
        {
            Id = Guid.NewGuid();
            Name = documentName;
            DocumentType = documentType;
        }

        // Needed for serialization
        protected DocumentMetadata(SerializationInfo info, StreamingContext context)
        {
            this.Id = (Guid)info.GetValue(IdSerializationName, typeof(Guid));
            this.Name = info.GetString(NameSerializationName);
            this.DocumentType = info.GetString(DocumentTypeSerializationName);
        }

        public static DocumentMetadata Create(string documentName, string documentType)
        {
            return new DocumentMetadata(documentName, documentType);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(IdSerializationName, this.Id);
            info.AddValue(NameSerializationName, this.Name);
            info.AddValue(DocumentTypeSerializationName, this.DocumentType);
        }
    }
}
