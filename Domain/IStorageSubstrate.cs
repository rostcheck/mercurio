using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents a space that can for physically hold data (in Containers). Respnsible for I/O operations
    /// but deals only in encrypted data.
    /// </summary>
    public interface IStorageSubstrate
    {
        string Name { get; }
		bool IsDefaultStorageSubstrate { get; } // Environment has one default substrate, where the keychain is store

        List<IContainer> GetAllContainers();
        IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne);
        //void StoreContainer(IContainer container);
        bool HostsContainer(Guid containerId);
        byte[] RetrievePrivateMetadataBytes(Guid containerId);
        void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion);
        DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata);
        void DeleteDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersion);
        void StoreMetadata(Guid containerId, ContainerMetadata metadata);
        void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata);
        void DeleteContainer(Guid containerId);
        byte[] RetrieveDatabase(Guid containerId, Guid databaseId);
        void StoreDatabase(Guid containerId, Guid databaseId, Stream encryptedDatabaseData);
        void DeleteDatabase(Guid containerId, Guid databaseId);
    }
}
