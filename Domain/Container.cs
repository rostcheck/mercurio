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
        protected IStorageSubstrate _substrate;
        protected Serializer _serializer;
        protected ICryptoManager _cryptoManager;
        protected ContainerMetadata _metadata;
        protected ContainerPrivateMetadata _privateMetadata;

        protected Container(string containerName, IStorageSubstrate substrate, Serializer serializer, ICryptoManager cryptoManager = null, RevisionRetentionPolicyType retentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            Id = Guid.NewGuid();
            _substrate = substrate;
            _serializer = serializer;
            _cryptoManager = cryptoManager;
            _metadata = ContainerMetadata.Create(containerName, cryptoManager.ManagerType, cryptoManager.GetActiveIdentity());
            _privateMetadata = ContainerPrivateMetadata.Create(containerName, "", retentionPolicyType);
            ChangeRevisionRetentionPolicy(retentionPolicyType);
        }

        protected Container(Guid id, ContainerMetadata metadata, IStorageSubstrate substrate, Serializer serializer)
        {
            Id = id;
            _metadata = metadata;
            _substrate = substrate;
            _serializer = serializer;
            _privateMetadata = null; // Created locked
        }

        // Container is created unlocked
        public static IContainer Create(string name, ICryptoManager cryptoManager, IStorageSubstrate substrate, Serializer serializer, RevisionRetentionPolicyType retentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            if (cryptoManager.GetActiveIdentity() == string.Empty)
                throw new MercurioExceptionIdentityNotSet("Identity not set on cryptoManager");

            var container = new Container(name, substrate, serializer, cryptoManager, retentionPolicyType);
            container.EnsureStorageRepresentationExists();
            return container;
        }

        private void EnsureStorageRepresentationExists()
        {
            StoreMetadata();
            StorePrivateMetadata();
        }

        /// <summary>
        /// Create Container from an existing stored representation. The container is locked (only public metadata is loaded)
        /// </summary>
        public static Container CreateFrom(ContainerMetadata metadata, Guid id, IStorageSubstrate substrate, Serializer serializer)
        {
            return new Container(id, metadata, substrate, serializer);
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

        public virtual void Lock(ICryptoManager cryptoManager)
        {
            if (IsLocked)
                return; // already locked

            StorePrivateMetadata();
            _privateMetadata = null; //TODO: Secure erase
        }

        private void StoreMetadata()
        {
            _substrate.StoreMetadata(this.Id, _metadata);
        }

        private void StorePrivateMetadata()
        {
            MemoryStream stream = new MemoryStream();
            _serializer.Serialize(stream, _privateMetadata);
            stream.Flush();
            stream.Position = 0;
            var encryptedStream = _cryptoManager.Encrypt(stream, _metadata.KeyFingerprint);
            stream.Close();
            encryptedStream.Position = 0;
            _substrate.StorePrivateMetadata(this.Id, encryptedStream);
        }

        /// <summary>
        /// Unlock the container (read its private metadata). Requires a cryptoManager w/ credentials set
        /// </summary>
        /// <param name="cryptoManager"></param>
        public virtual void Unlock(byte[] privateMetadataBytes, ICryptoManager cryptoManager)
        {
            _privateMetadata = cryptoManager.Decrypt<ContainerPrivateMetadata>(privateMetadataBytes, _serializer);
            _cryptoManager = cryptoManager;
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
            StorePrivateMetadata();
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

        public DocumentVersion GetDocumentVersion(DocumentVersionMetadata versionMetadata, bool metadataOnly = false)
        {
            if (versionMetadata == null)
                throw new ArgumentNullException();

            VerifyIsUnlocked();

            var documentVersionMetadata = _privateMetadata.GetSpecificVersion(versionMetadata.DocumentId, versionMetadata.Id);
            if (documentVersionMetadata == null)
                throw new MercurioException("Document version not found");

            var documentVersion = _substrate.RetrieveDocumentVersion(this.Id, documentVersionMetadata);
            var unencryptedDocumentContent = (metadataOnly == true) ? null : _cryptoManager.Decrypt(documentVersion.DocumentContent);
            return DocumentVersion.CreateWithUnencryptedContent(documentVersion, unencryptedDocumentContent);
        }

        public DocumentVersion GetLatestDocumentVersion(string documentName, bool metadataOnly = false)
        {
            if (string.IsNullOrEmpty(documentName))
                throw new ArgumentException("documentName");

            VerifyIsUnlocked();
            var availableVersions = _privateMetadata.GetAvailableVersions(documentName);
            if (availableVersions == null)
                return null;
            var documentVersionMetadata = availableVersions.OrderByDescending(s => s.CreatedDateTime).First();
            var documentVersion = _substrate.RetrieveDocumentVersion(this.Id, documentVersionMetadata);
            var unencryptedDocumentContent = (metadataOnly == true) ? null : _cryptoManager.Decrypt(documentVersion.DocumentContent);
            return DocumentVersion.CreateWithUnencryptedContent(documentVersion, unencryptedDocumentContent);
        }

        public virtual DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData)
        {
            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion != null)
                throw new MercurioException(string.Format("Document {0} already exists in this container", documentName));

            VerifyIsUnlocked();

            var documentMetadata = DocumentMetadata.Create(documentName);
            var encryptedInitialData = _cryptoManager.Encrypt(initialData, creatorIdentity.UniqueIdentifier);
            var documentVersion = DocumentVersion.Create(documentMetadata.Id, Guid.Empty, 0, creatorIdentity.UniqueIdentifier, encryptedInitialData);

            _substrate.StoreDocumentVersion(this.Id, documentVersion);
            _privateMetadata.AddDocumentVersion(documentName, documentVersion.Metadata);
            StorePrivateMetadata();            
            return DocumentVersion.CreateWithUnencryptedContent(documentVersion, initialData);
        }

        private Guid GetDocumentId(string documentName)
        {
            VerifyIsUnlocked();
            return _privateMetadata.GetDocumentId(documentName);
        }

        public virtual DocumentVersion ModifyTextDocument(string documentName, Identity modifierIdentity, string modifiedData)
        {
            VerifyIsUnlocked();

            var documentId = GetDocumentId(documentName);
            if (documentId == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));

            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion == null)
                throw new MercurioException(string.Format("Document {0} does not have any versions in this container", documentName)); // internal inconsistency

            var encryptedData = _cryptoManager.Encrypt(modifiedData, modifierIdentity.UniqueIdentifier);
            var newVersion = DocumentVersion.Create(documentId, latestVersion.Id, latestVersion.CreatedDateTime.UtcTicks, modifierIdentity.UniqueIdentifier, encryptedData);

            _substrate.StoreDocumentVersion(this.Id, newVersion);
            _privateMetadata.AddDocumentVersion(documentName, newVersion.Metadata);
            StorePrivateMetadata();
            return newVersion;
        }

        public virtual DocumentVersion DeleteDocumentSoft(string documentName, Identity modifierIdentity)
        {
            VerifyIsUnlocked();

            var documentId = GetDocumentId(documentName);
            if (documentId == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));

            var latestVersion = GetLatestDocumentVersion(documentName, true);
            if (latestVersion == null)
                throw new MercurioException(string.Format("Document {0} does not have any versions in this container", documentName)); // internal inconsistency

            var newVersion = DocumentVersion.CreateDeleted(documentId, latestVersion.Id, latestVersion.CreatedDateTime.UtcTicks, modifierIdentity.UniqueIdentifier);

            _substrate.StoreDocumentVersion(this.Id, newVersion);
            _privateMetadata.AddDocumentVersion(documentName, newVersion.Metadata);
            StorePrivateMetadata();
            return newVersion;
        }

        public virtual DocumentVersion UnDeleteDocument(string documentName, Identity modifierIdentity)
        {
            VerifyIsUnlocked();

            var documentId = GetDocumentId(documentName);
            if (documentId == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));

            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion == null)
                throw new MercurioException(string.Format("Document {0} does not have any versions in this container", documentName)); // internal inconsistency

            if (!latestVersion.Metadata.IsDeleted)
                throw new MercurioException(string.Format("Document {0} is not marked as deleted", documentName));

            // Delete the empty version that serves as a delete marker
            _substrate.DeleteDocumentVersion(this.Id, latestVersion.Metadata);
            _privateMetadata.RemoveDocumentVersion(documentName, latestVersion.Metadata);
            StorePrivateMetadata();
            return GetLatestDocumentVersion(documentName);
        }

        public virtual void DeleteDocumentHard(string documentName, Identity modifierIdentity)
        {
            VerifyIsUnlocked();

            var documentId = GetDocumentId(documentName);
            if (documentId == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));

            var versions = ListAvailableVersions(documentName);
            foreach (var version in versions)
            {
                try
                {
                    _substrate.DeleteDocumentVersion(this.Id, version);
                }
                catch (Exception) { } // Suppress errors deleting physical records; we're about to delete their metadata
            }
            _privateMetadata.RemoveDocument(documentName);

            StorePrivateMetadata();
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

        private void VerifyIsUnlocked()
        {
            if (_privateMetadata == null)
            {
                throw new UnauthorizedAccessException("Container is locked");
            }
        }

        public bool ContainsDocument(string documentName)
        {
            VerifyIsUnlocked();

            documentName = documentName.ToLower();
            foreach (var containedDocument in _privateMetadata.GetAvailableDocuments())
            {
                if (documentName == containedDocument.ToLower())
                    return true;
            }
            return false;
        }
    }
}
