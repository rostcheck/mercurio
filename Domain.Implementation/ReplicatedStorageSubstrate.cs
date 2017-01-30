using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
	public class ReplicatedStorageSubstrate : StorageSubstrateBase, IStorageSubstrate
	{
		private IStorageSubstrate _baseSubstrate;
		private IPersistentQueue _messageQueue;

		public ReplicatedStorageSubstrate(IStorageSubstrate baseSubstrate, IPersistentQueue messageQueue, bool isDefault)
			: base(isDefault)
		{
			this._baseSubstrate = baseSubstrate;
			this._messageQueue = messageQueue;
		}

		#region IStorageSubstrate implementation

		public List<IContainer> GetAllContainers()
		{
			return _baseSubstrate.GetAllContainers();
		}

		public IContainer CreateContainer(string containerName, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
		{
			var container = _baseSubstrate.CreateContainer(containerName, cryptoManager, retentionPolicy);
			//_messageQueue.Add(new CreateContainerMessage(containerName)); // implement
			return container;
		}

		public bool HostsContainer(Guid containerId)
		{
			return _baseSubstrate.HostsContainer(containerId);
		}

		public byte[] RetrievePrivateMetadataBytes(Guid containerId)
		{
			return _baseSubstrate.RetrievePrivateMetadataBytes(containerId);
		}

		public void StoreDocumentVersion(Guid containerId, DocumentVersion documentVersion)
		{
			_baseSubstrate.StoreDocumentVersion(containerId, documentVersion);
			//_messageQueue.Add(new StoreDocumentVersionMessage(containerId, documentVersion)); // Implement
		}

		public DocumentVersion RetrieveDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersionMetadata)
		{
			return _baseSubstrate.RetrieveDocumentVersion(containerId, documentVersionMetadata);
		}

		public void DeleteDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersion)
		{
			_baseSubstrate.DeleteDocumentVersion(containerId, documentVersion);
			//_messageQueue.Add(new DeleteDocumentMessage(containerId, documentVersion)); // Implement
		}

		public void StoreMetadata(Guid containerId, ContainerMetadata metadata)
		{
			_baseSubstrate.StoreMetadata(containerId, metadata);
			//_messageQueue.Add(new StoreMetadataMessage(containerId, metadata)); // Implement
		}

		public void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata)
		{
			_baseSubstrate.StorePrivateMetadata(containerId, encryptedPrivateMetadata);
			//_messageQueue.Add(new StorePrivateMetadataMessage(containerId, encryptedPrivateMetadata)); // Implement			
		}

		public void DeleteContainer(Guid containerId)
		{
			_baseSubstrate.DeleteContainer(containerId);
			//_messageQueue.Add(new DeleteContainerMessage(containerId));
		}

        public byte[] RetrieveDatabase(Guid containerId, Guid databaseId)
        {
            return _baseSubstrate.RetrieveDatabase(containerId, databaseId);
        }

        public void StoreDatabase(Guid containerId, Guid databaseId, Stream encryptedDatabaseData)
        {
            _baseSubstrate.StoreDatabase(containerId, databaseId, encryptedDatabaseData);
            //_messageQueue.Add(new StoreDatabaseMessage(containerId, databaseId, encryptedDatabaseData));
        }

        public void DeleteDatabase(Guid containerId, Guid databaseId)
        {
            _baseSubstrate.DeleteDatabase(containerId, databaseId);
            //_messageQueue.Add(new DeleteDatabaseMessage(containerId, databaseId));
        }

        public string Name
		{
			get
			{
				return _baseSubstrate.Name + "-replicated";
			}
		}

		#endregion
	}
}

