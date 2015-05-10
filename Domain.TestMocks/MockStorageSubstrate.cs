using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.TestMocks
{
    public class MockStorageSubstrate : IStorageSubstrate
    {
        private List<IContainer> _containers;

        public MockStorageSubstrate()
        {
            _containers = new List<IContainer>();
        }

        public string Name
        {
            get { return "MockStorageSubstate"; }
        }

        public IEnumerable<IContainer> GetAllContainers()
        {
            return new List<IContainer>(_containers);
        }

        public bool HostsContainer(string containerId)
        {
            throw new NotImplementedException();
        }

        public byte[] RetrievePrivateMetadataBytes(Guid containerId)
        {
            throw new NotImplementedException();
        }


        public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy)
        {
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var container = Container.Create(containerName, cryptoManager, this, serializer, retentionPolicy);
            _containers.Add(container);
            return container;
        }

        public bool HostsContainer(Guid containerId)
        {
            throw new NotImplementedException();
        }

        public void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion)
        {
            throw new NotImplementedException();
        }

        public DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
        {
            throw new NotImplementedException();
        }

        public void StoreMetadata(Guid containerId, ContainerMetadata metadata)
        {
            return;
        }

        public void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata)
        {
            return;
        }

        public void DeleteContainer(Guid containerId)
        {
            return;
        }
    }
}
