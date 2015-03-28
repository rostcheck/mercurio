using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents a space that can for physically hold data (in Containers). Respnsible for I/O operations
    /// but deals only in unsecure or encrypted data.
    /// </summary>
    public interface IStorageSubstrate
    {
        string Name { get; }

        IEnumerable<IContainer> GetAllContainers();
        IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy);
        //void StoreContainer(IContainer container);
        bool HostsContainer(string containerId);
        byte[] GetPrivateMetadataBytes(string containerId);
    }
}
