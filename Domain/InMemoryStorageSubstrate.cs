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

        public IEnumerable<IContainer> GetContainers()
        {
            throw new NotImplementedException();
        }

        public IContainer CreateContainer(string containerName, IStoragePlan storagePlan, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return Container.Create(containerName, storagePlan, retentionPolicy);
        }
    }
}
