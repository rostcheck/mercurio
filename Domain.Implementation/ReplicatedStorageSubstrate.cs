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

		public ReplicatedStorageSubstrate(IStorageSubstrate baseSubstrate, IPersistentQueue messageQueue)
		{
			this._baseSubstrate = baseSubstrate;
			this._messageQueue = messageQueue;
		}

		#region IStorageSubstrate implementation

		public List<IContainer> GetAllContainers()
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

		public void DeleteDocumentVersion(Guid containerId, DocumentVersionMetadata documentVersion)
		{
			throw new NotImplementedException();
		}

		public void StoreMetadata(Guid containerId, ContainerMetadata metadata)
		{
			throw new NotImplementedException();
		}

		public void StorePrivateMetadata(Guid containerId, Stream encryptedPrivateMetadata)
		{
			throw new NotImplementedException();
		}

		public void DeleteContainer(Guid containerId)
		{
			throw new NotImplementedException();
		}

		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

