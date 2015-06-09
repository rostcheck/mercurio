using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public class InMemoryStorageSubstrate : IStorageSubstrate
    {
        private Dictionary<Guid, DocumentVersion> _documentVersions;
        private Dictionary<Guid, ContainerMetadata> _metadata;
        private Dictionary<Guid, byte[]> _privateMetadata;

        public InMemoryStorageSubstrate()
        {
            _documentVersions = new Dictionary<Guid, DocumentVersion>();
            _metadata = new Dictionary<Guid, ContainerMetadata>();
            _privateMetadata = new Dictionary<Guid, byte[]>();
        }

        public string Name
        {
            get { return "Memory"; }
        }

        public List<IContainer> GetAllContainers()
        {
            throw new NotImplementedException();
        }


        public byte[] RetrievePrivateMetadataBytes(Guid containerId)
        {
            return (_privateMetadata.ContainsKey(containerId)) ? _privateMetadata[containerId] : null;
        }

        public bool HostsContainer(Guid containerId)
        {
            return _metadata.ContainsKey(containerId);
        }

        public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            return Container.Create(containerName, cryptoManager, this, serializer, retentionPolicy);
        }

        public void DeleteContainer(Guid containerId)
        {
            throw new NotImplementedException();
        }

        public void StoreContainer(IContainer container)
        {
            throw new NotImplementedException();
        }

        public void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion)
        {
            _documentVersions.Add(documentVersion.Id, documentVersion);
        }

        public DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
        {
            return _documentVersions[documentVersionMetadata.Id];
        }

        public void StoreMetadata(Guid containerId, ContainerMetadata metadata)
        {
            _metadata[containerId] = metadata;
        }

        public void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata)
        {
            using (var memoryStream = new MemoryStream())
            {
                encryptedPrivateMetadata.Position = 0;
                encryptedPrivateMetadata.CopyTo(memoryStream);
                _privateMetadata[containerId] = memoryStream.ToArray();
            }
        }
    }
}
