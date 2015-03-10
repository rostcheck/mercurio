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

        public IContainer CreateContainer(string containerName, string keyFingerprint, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return Container.Create(containerName, cryptoManager, retentionPolicy);
        }

        public IEnumerable<IContainer> GetAccessibleContainers(string identity, ICryptoManager cryptoManager)
        {
            throw new NotImplementedException();
        }
    }
}
