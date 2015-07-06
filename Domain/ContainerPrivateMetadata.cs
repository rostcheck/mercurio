using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Metadata about the Container that is encrypted when stored onto the StorageSubstrate
    /// </summary>
    [Serializable]
    public class ContainerPrivateMetadata : ISerializable
    {
        private const string ContainerNameSerializationName = "ContainerName";
        private const string ContainerDescriptionSerializationName = "ContainerDescription";
        private const string DocumentDirectorySerializationName = "DocumentDirectory";
        private const string VersionDirectorySerializationName = "DocumentVersionDirectory";
        private const string RevisionRetentionPolicyTypeSerializationName = "RevisionRetentionPolicyType"; //TODO: move to private metadata

        public string ContainerName { get; private set; }
        public string ContainerDescription { get; private set; }
        public int RevisionRetentionPolicyType { get; set; }
        private Dictionary<string, DocumentMetadata> _documentDirectory; // By document name
        private Dictionary<Guid, List<DocumentVersionMetadata>> _documentVersionDirectory; // By document id

        // Needed for serialization
        protected ContainerPrivateMetadata(SerializationInfo info, StreamingContext context)
        {
            this.ContainerName = info.GetString(ContainerNameSerializationName);
            this.ContainerDescription = info.GetString(ContainerDescriptionSerializationName);
            this._documentVersionDirectory = info.GetValue(VersionDirectorySerializationName, typeof(Dictionary<Guid, List<DocumentVersionMetadata>>)) as Dictionary<Guid, List<DocumentVersionMetadata>>;
            this._documentDirectory = info.GetValue(DocumentDirectorySerializationName, typeof(Dictionary<string, DocumentMetadata>)) as Dictionary<string, DocumentMetadata>;
            this.RevisionRetentionPolicyType = info.GetInt32(RevisionRetentionPolicyTypeSerializationName);
        }

        public ContainerPrivateMetadata(string name, string description, RevisionRetentionPolicyType retentionPolicyType)
        {
            this.ContainerName = name;
            this.ContainerDescription = description;
            _documentDirectory = new Dictionary<string, DocumentMetadata>();
            _documentVersionDirectory = new Dictionary<Guid, List<DocumentVersionMetadata>>();
            this.RevisionRetentionPolicyType = (int)retentionPolicyType;
        }

        public static ContainerPrivateMetadata Create(string name, string description, RevisionRetentionPolicyType retentionPolicyType)
        {
            return new ContainerPrivateMetadata(name, description, retentionPolicyType);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ContainerNameSerializationName, ContainerName);
            info.AddValue(ContainerDescriptionSerializationName, ContainerDescription);
            info.AddValue(DocumentDirectorySerializationName, _documentDirectory);
            info.AddValue(VersionDirectorySerializationName, _documentVersionDirectory);
            info.AddValue(RevisionRetentionPolicyTypeSerializationName, RevisionRetentionPolicyType);
        }

        public ICollection<string> GetAvailableDocuments()
        {
            return _documentDirectory.Keys.ToList();
        }

        private DocumentMetadata GetDocumentMetadata(string documentName)
        {
            return _documentDirectory.ContainsKey(documentName) ? _documentDirectory[documentName] : null;
        }

        public Guid GetDocumentId(string documentName)
        {
            var documentMetadata = GetDocumentMetadata(documentName);
            return documentMetadata == null ? Guid.Empty : documentMetadata.Id;
        }

        public ICollection<DocumentVersionMetadata> GetAvailableVersions(string documentName)
        {
            var documentMetadata = GetDocumentMetadata(documentName);
            return documentMetadata == null ? null : GetAvailableVersions(documentMetadata.Id);
        }

        public ICollection<DocumentVersionMetadata> GetAvailableVersions(Guid documentId)
        {
            return _documentVersionDirectory.ContainsKey(documentId) ? new List<DocumentVersionMetadata>(_documentVersionDirectory[documentId]) : null;
        }

        public DocumentVersionMetadata GetSpecificVersion(Guid documentId, Guid documentVersionId)
        {
            if (!_documentVersionDirectory.ContainsKey(documentId))
                return null;

            return _documentVersionDirectory[documentId].Where(s => s.Id == documentVersionId).FirstOrDefault();
        }

        public void AddDocumentVersion(string documentName, DocumentVersionMetadata documentVersionMetadata)
        {
            var documentMetadata = _documentDirectory.ContainsKey(documentName) ? GetDocumentMetadata(documentName) : null;

            // Create directory entry if it doesn't exist
            if (documentMetadata == null)
            {
                documentMetadata = DocumentMetadata.Create(documentName);
                _documentDirectory.Add(documentName, documentMetadata);
                _documentVersionDirectory.Add(documentMetadata.Id, new List<DocumentVersionMetadata>());
            }

            _documentVersionDirectory[documentMetadata.Id].Add(documentVersionMetadata);
            // Apply version retention policy
            _documentVersionDirectory[documentMetadata.Id] = _documentVersionDirectory[documentMetadata.Id].WithoutExcessRevisions((RevisionRetentionPolicyType)this.RevisionRetentionPolicyType);          
        }

        public void RemoveDocumentVersion(string documentName, DocumentVersionMetadata documentVersionMetadata)
        {            
            var documentMetadata = _documentDirectory.ContainsKey(documentName) ? GetDocumentMetadata(documentName) : null;
            if (documentMetadata == null)
                throw new MercurioException(string.Format("Directory does not contain an entry for document {0}", documentName));

            var documentVersion = _documentVersionDirectory[documentMetadata.Id].Where(s => s.Id == documentVersionMetadata.Id).FirstOrDefault();
            if (documentVersion == null)
                throw new MercurioException(string.Format("Document version list does not contain an entry for document {0} version {1}", documentName, documentVersionMetadata.Id));

            _documentVersionDirectory[documentMetadata.Id].Remove(documentVersion);           
        }

        public void RemoveDocument(string documentName)
        {
            if (!_documentDirectory.ContainsKey(documentName))
                throw new MercurioException("No such file exists");

            var documentMetadata = GetDocumentMetadata(documentName);
            _documentVersionDirectory.Remove(documentMetadata.Id);
            _documentDirectory.Remove(documentName);
        }
    }
}
