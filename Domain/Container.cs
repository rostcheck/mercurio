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

        protected Container(string containerName, ICryptoManager cryptoManager = null, RevisionRetentionPolicyType retentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            Id = Guid.NewGuid().ToString();
            _cryptoManager = cryptoManager;
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
        public static Container Create(string name, ICryptoManager cryptoManager, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return new Container(name, cryptoManager, retentionPolicy);
        }

        public virtual string Id { get; protected set; }

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
        public virtual void Unlock(ICryptoManager cryptoManager)
        {
            //_privateMetadata = cryptoManager.Decrypt<ContainerPrivateMetadata>(privateMetadataBytes, serializer);
            throw new NotImplementedException();
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

        public ICollection<DocumentVersionMetadata> GetAvailableVersions(string documentName)
        {
            VerifyIsUnlocked();
            return _privateMetadata.GetAvailableVersions(documentName);
        }

        public DocumentVersionMetadata GetLatestDocumentVersion(string documentName)
        {
            VerifyIsUnlocked();
            var availableVersions = _privateMetadata.GetAvailableVersions(documentName);
            return (availableVersions == null) ? null : availableVersions.OrderByDescending(s => s.CreatedDateTime).First();
        }

        public virtual DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData)
        {
            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion != null)
                throw new MercurioException(string.Format("Document {0} already exists in this container", documentName));

            var document = DocumentVersion.Create(Guid.Empty, creatorIdentity.UniqueIdentifier, initialData);

            var newVersion = _privateMetadata.CreateDocumentVersion(documentName, creatorIdentity.UniqueIdentifier);
            var textDocument = StoreDocumentVersion(documentName, newVersion, initialData);
            _privateMetadata.AddDocumentVersion(documentName, newVersion);
            return textDocument;
        }

        public virtual DocumentVersion ModifyTextDocument(string documentName, Identity modifierIdentity, string modifiedData)
        {
            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));

            var document = DocumentVersion.Create(latestVersion.Id, modifierIdentity.UniqueIdentifier, modifiedData);

            var newVersion = _privateMetadata.CreateDocumentVersion(documentName, modifierIdentity.UniqueIdentifier);
            var textDocument = StoreDocumentVersion(documentName, newVersion, modifiedData);
            _privateMetadata.AddDocumentVersion(documentName, newVersion);
            return textDocument;
        }

        public virtual DocumentVersion RetrieveDocument(string documentName)
        {
            return RetrieveDocument(documentName, Guid.Empty);
        }

        public virtual DocumentVersion RetrieveDocument(string documentName, Guid specificVersionId)
        {          
            var latestVersion = GetLatestDocumentVersion(documentName);
            if (latestVersion == null)
                throw new MercurioException(string.Format("Document {0} does not exist in this container", documentName));
            if (specificVersionId == Guid.Empty)
                specificVersionId = latestVersion.Id;
            else
            {
                if (_privateMetadata.GetAvailableVersions(documentName).Where(s => s.Id == specificVersionId).Count() < 1)
                    throw new MercurioException(string.Format("Version {0} of doument {1} was not found in this container", specificVersionId.ToString(), documentName));
            }
            return RetrieveDocumentVersion(documentName, specificVersionId);
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

        protected virtual DocumentVersion StoreDocumentVersion(string documentName, DocumentVersionMetadata newVersion, string initialData)
        {
            throw new NotImplementedException(); // Defer to storage substrate
        }

        protected virtual DocumentVersion RetrieveDocumentVersion(string documentName, Guid versionId)
        {
            throw new NotImplementedException(); // Defer to storage substrate
        }

        private void VerifyIsUnlocked()
        {
            if (_privateMetadata == null)
            {
                throw new UnauthorizedAccessException("Container is locked");
            }
        }
    }
}
