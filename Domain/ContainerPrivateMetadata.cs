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
        private const string DirectorySerializationName = "Directory";
        private const string RevisionRetentionPolicyTypeSerializationName = "RevisionRetentionPolicyType"; //TODO: move to private metadata

        public string ContainerName { get; private set; }
        public string ContainerDescription { get; private set; }
        public int RevisionRetentionPolicyType { get; set; }
        private Dictionary<string, List<DocumentVersionMetadata>> _directory;

        // Needed for serialization
        protected ContainerPrivateMetadata(SerializationInfo info, StreamingContext context)
        {
            this.ContainerName = info.GetString(ContainerNameSerializationName);
            this.ContainerDescription = info.GetString(ContainerDescriptionSerializationName);
            this._directory = info.GetValue(DirectorySerializationName, typeof(Dictionary<string, List<DocumentVersionMetadata>>)) as Dictionary<string, List<DocumentVersionMetadata>>;
            this.RevisionRetentionPolicyType = info.GetInt32(RevisionRetentionPolicyTypeSerializationName);
        }

        public ContainerPrivateMetadata(string name, string description, RevisionRetentionPolicyType retentionPolicyType)
        {
            this.ContainerName = name;
            this.ContainerDescription = description;
            _directory = new Dictionary<string, List<DocumentVersionMetadata>>();
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
            info.AddValue(DirectorySerializationName, _directory);
            info.AddValue(RevisionRetentionPolicyTypeSerializationName, RevisionRetentionPolicyType);
        }

        public ICollection<string> GetAvailableDocuments()
        {
            return _directory.Keys.ToList();
        }

        public ICollection<DocumentVersionMetadata> GetAvailableVersions(string documentName)
        {
            return _directory.ContainsKey(documentName) ? new List<DocumentVersionMetadata>(_directory[documentName]) : null;
        }


        public DocumentVersionMetadata CreateDocumentVersion(string documentName, string revisorId)
        {
            if (!_directory.ContainsKey(documentName))
                _directory.Add(documentName, new List<DocumentVersionMetadata>());

            var lastVersion = _directory[documentName].OrderBy(s => s.CreatedDateTime).FirstOrDefault();
            Guid parentVersionId = (lastVersion == null) ? Guid.Empty : lastVersion.Id;
            return DocumentVersionMetadata.Create(parentVersionId, revisorId);
        }

        public void AddDocumentVersion(string documentName, DocumentVersionMetadata version)
        {           
            _directory[documentName].Add(version);
            // Apply version retention policy
            _directory[documentName] = _directory[documentName].WithoutExcessRevisions((RevisionRetentionPolicyType)this.RevisionRetentionPolicyType);
        }

        public void DeleteFile(string documentName)
        {
            _directory.Remove(documentName);
        }

    }
}
