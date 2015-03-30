using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public class InMemoryStorageSubstrate : IStorageSubstrate
    {
        private Dictionary<Guid, DocumentVersion> _documentVersions;

        public InMemoryStorageSubstrate()
        {
            _documentVersions = new Dictionary<Guid, DocumentVersion>();
        }
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IContainer> GetAllContainers()
        {
            throw new NotImplementedException();
        }


        public byte[] GetPrivateMetadataBytes(string containerId)
        {
            throw new NotImplementedException();
        }

        public bool HostsContainer(string containerId)
        {
            throw new NotImplementedException();
        }


        public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            var container = Container.Create(containerName, cryptoManager, retentionPolicy);
            container.RetrieveDocumentVersionEvent += container_RetrieveDocumentVersionEvent;
            container.StoreDocumentVersionEvent += container_StoreDocumentVersionEvent;
            return container;
        }

        void container_StoreDocumentVersionEvent(Guid containerId, DocumentVersion documentVersion)
        {
            _documentVersions.Add(documentVersion.Id, documentVersion);
        }

        DocumentVersion container_RetrieveDocumentVersionEvent(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
        {
            return _documentVersions[documentVersionMetadata.Id];
        }

        public void StoreContainer(IContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
