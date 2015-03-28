using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public class InMemoryStorageSubstrate : IStorageSubstrate
    {
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


        public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy)
        {
            throw new NotImplementedException();
        }

        public void StoreContainer(IContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
