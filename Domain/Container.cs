using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A collection of Records (basic container)
    /// </summary>
    public class Container : IContainer
    {
        protected ICryptoManager _cryptoManager;
        protected ContainerMetadata _metadata;
        protected ContainerPrivateMetadata _privateMetadata;
        protected IStorageSubstrate _storageSubstrate;

        protected Container(string containerName, IStorageSubstrate storageSubstrate, ICryptoManager cryptoManager = null, RevisionRetentionPolicyType retentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            Id = Guid.NewGuid();
            _cryptoManager = cryptoManager;
            _storageSubstrate = storageSubstrate;
            _metadata = ContainerMetadata.Create(containerName, cryptoManager.ManagerType, cryptoManager.GetActiveIdentity());
            _privateMetadata = ContainerPrivateMetadata.Create(containerName, "", retentionPolicyType);
            ChangeRevisionRetentionPolicy(retentionPolicyType);
        }

        protected Container(ContainerMetadata metadata)
        {
            _metadata = metadata;
            _privateMetadata = null; // Created locked
        }

        // Container is created unlocked
        public static Container Create(IStorageSubstrate storageSubstrate, string name, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            if (cryptoManager.GetActiveIdentity() == string.Empty)
                throw new MercurioExceptionIdentityNotSet("Identity not set on cryptoManager");

            return new Container(name, storageSubstrate, cryptoManager, retentionPolicyType);
        }


        public static IEnumerable<IContainer> GetAllContainers(IStorageSubstrate storageSubstate)
        {
            return storageSubstate.GetAllContainers();
        }

         // cryptoManager must have credential set
        public static IEnumerable<IContainer> GetAccessibleContainers(IStorageSubstrate storageSubstrate, string identity, string cryptoManagerType)
        {
            return storageSubstrate.GetAllContainers().Where(s => (s.CryptoManagerType == cryptoManagerType && s.IsAvailableToIdentity(identity)));
        }

        /// <summary>
        /// Create Container from an existing stored representation. The container is locked (only public metadata is loaded)
        /// </summary>
        public static Container CreateFrom(ContainerMetadata metadata)
        {
            return new Container(metadata);
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name
        {
            get
            {
                return(_metadata == null) ? string.Empty : _metadata.Name;
            }
            protected set
            {
                if (_metadata != null)
                {
                    _metadata.Name = value;
                }
            }
        }

        public virtual string CryptoManagerType
        {
            get
            {
                return (_metadata == null) ? string.Empty : _metadata.CryptoProviderType;
            }
        }

        public virtual bool IsLocked
        {
            get
            {
                return _privateMetadata == null;
            }
        }

        public virtual void Lock()
        {
            _privateMetadata = null; //TODO: Secure erase
        }

        /// <summary>
        /// Unlock the container (read its private metadata). Requires a cryptoManager w/ credentials set
        /// </summary>
        /// <param name="cryptoManager"></param>
        public virtual void Unlock(byte[] privateMetadataBytes, ICryptoManager cryptoManager, Serializer serializer)
        {
            _privateMetadata = cryptoManager.Decrypt<ContainerPrivateMetadata>(privateMetadataBytes, serializer);
        }

        public virtual bool IsAvailableToIdentity(string uniqueIdentifier)
        {
            return (_metadata.KeyFingerprint == uniqueIdentifier); // TODO: generalize to support multiple identities
        }

        public virtual void AddIdentity(Identity identity, AccessPermissionType accessPermissionType)
        {
            throw new NotImplementedException();
        }

        public virtual IRevisionRetentionPolicy RevisionRetentionPolicy
        {
            get
            {
                VerifyIsUnlocked();
                return Mercurio.Domain.RevisionRetentionPolicy.Create((RevisionRetentionPolicyType)_privateMetadata.RevisionRetentionPolicyType);
            }
        }

        public virtual void ChangeRevisionRetentionPolicy(RevisionRetentionPolicyType revisionRetentionPolicyType)
        {
            VerifyIsUnlocked();
            _privateMetadata.RevisionRetentionPolicyType = (int)revisionRetentionPolicyType;
        }

        public ICollection<string> Documents
        {
            get
            {
                VerifyIsUnlocked();
                return _privateMetadata.GetAvailableDocuments();
            }
        }

        public ICollection<DocumentVersionMetadata> ListAvailableVersions(string documentName)
        {
            VerifyIsUnlocked();
            return _privateMetadata.GetAvailableVersions(documentName);
        }

        public DocumentVersion GetDocumentVersion(DocumentVersionMetadata versionMetadata)
        {
            if (versionMetadata == null)
                throw new ArgumentNullException();

            var documentVersionMetadata = _privateMetadata.GetSpecificVersion(versionMetadata.DocumentId, versionMetadata.Id);
            if (documentVersionMetadata == null)
                throw new MercurioException("Document version not found");

            return RetrieveDocumentVersion(documentVersionMetadata);
        }

        public DocumentVersion GetLatestDocumentVersion(string documentName)
        {
            if (string.IsNullOrEmpty(documentName))
                throw new ArgumentException("documentName");

            VerifyIsUnlocked();
            var availableVersions = _privateMetadata.GetAvailableVersions(documentName);
            if (availableVersions == null)
                return null;

            var documentVersionMetadata = availableVersions.OrderByDescending(s => s.CreatedDateTime).First();
            return RetrieveDocumentVersion(documentVersionMetadata);
        }

        public virtual DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData)
        {
            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion != null)
                throw new MercurioException(string.Format("Document {0} already exists in this container", documentName));

            var documentMetadata = DocumentMetadata.Create(documentName);
            var documentVersion = DocumentVersion.Create(documentMetadata.Id, Guid.Empty, creatorIdentity.UniqueIdentifier, initialData);

            StoreDocumentVersion(documentVersion);
            _privateMetadata.AddDocumentVersion(documentName, documentVersion.Metadata);
            return documentVersion;
        }

        private Guid GetDocumentId(string documentName)
        {
            VerifyIsUnlocked();
            return _privateMetadata.GetDocumentId(documentName);
        }

        public virtual DocumentVersion ModifyTextDocument(string documentName, Identity modifierIdentity, string modifiedData)
        {
            var documentId = GetDocumentId(documentName);
            if (documentId == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));

            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion == null)
                throw new MercurioException(string.Format("Document {0} does not have any versions in this container", documentName)); // internal inconsistency

            var newVersion = DocumentVersion.Create(documentId, latestVersion.Id, modifierIdentity.UniqueIdentifier, modifiedData);

            StoreDocumentVersion(newVersion);
            _privateMetadata.AddDocumentVersion(documentName, newVersion.Metadata);
            return newVersion;
        }

        public virtual void DeleteRecord(string recordId)
        {
            VerifyIsUnlocked();
            throw new NotImplementedException();
        }

        public virtual void ChangeRecord(Record changedRecord)
        {
            VerifyIsUnlocked();
            throw new NotImplementedException();
        }

        protected virtual void StoreDocumentVersion(DocumentVersion documentVersion)
        {
            // Defer to storage substrate
            if (StoreDocumentVersionEvent != null)
                StoreDocumentVersionEvent(this.Id, documentVersion);
        }

        protected virtual DocumentVersion RetrieveDocumentVersion(DocumentVersionMetadata documentVersionMetadata)
        {
            // Inform storage substrate we need it
            return (RetrieveDocumentVersionEvent != null) ? RetrieveDocumentVersionEvent(this.Id, documentVersionMetadata) : null;
        }

        private void VerifyIsUnlocked()
        {
            if (_privateMetadata == null)
            {
                throw new UnauthorizedAccessException("Container is locked");
            }
        }

        #region Events
        public event StoreDocumentVersionHandler StoreDocumentVersionEvent;
        public event RetrieveDocumentVersionHandler RetrieveDocumentVersionEvent;
        #endregion
    }
}
