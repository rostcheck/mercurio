using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell.UnitTests
{
    public class MockStorageSubstrate : IStorageSubstrate
    {

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IContainer> GetAllContainers()
        {
            throw new NotImplementedException();
        }

        public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            throw new NotImplementedException();
        }

        public bool HostsContainer(Guid containerId)
        {
            throw new NotImplementedException();
        }

        public byte[] RetrievePrivateMetadataBytes(Guid containerId)
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
            throw new NotImplementedException();
        }

        public void StorePrivateMetadata(Guid containerId, System.IO.Stream encryptedPrivateMetadata)
        {
            throw new NotImplementedException();
        }
    }


}
