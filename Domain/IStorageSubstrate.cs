using System;
using System.Collections.Generic;
using System.IO;
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
        IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne);
        //void StoreContainer(IContainer container);
        bool HostsContainer(Guid containerId);
        byte[] RetrievePrivateMetadataBytes(Guid containerId);
        void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion);
        DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata);
        void StoreMetadata(Guid containerId, ContainerMetadata metadata);
        void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata);
        void DeleteContainer(Guid containerId);
    }
}
