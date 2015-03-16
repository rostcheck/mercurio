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
        private List<IDocument> _documents;

        protected Container(string containerName, ICryptoManager cryptoManager = null, RevisionRetentionPolicyType retentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            Id = Guid.NewGuid().ToString();
            _documents = new List<IDocument>();
            _cryptoManager = cryptoManager;
            _metadata = ContainerMetadata.Create(containerName, cryptoManager.ManagerType, cryptoManager.GetActiveIdentity(), retentionPolicyType);
            _privateMetadata = ContainerPrivateMetadata.Create(containerName, "");
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

        public List<IDocument> Documents
        {
            get
            {
                VerifyIsUnlocked();
                return new List<IDocument>(_documents);
            }
            private set
            {
                VerifyIsUnlocked();
                _documents = value;
            }
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

        public virtual IRevisionRetentionPolicy RevisionRetentionPolicy
        {
            get
            {
                return Mercurio.Domain.RevisionRetentionPolicy.Create((RevisionRetentionPolicyType)_metadata.RevisionRetentionPolicyType);
            }
        }

        public virtual void ChangeRevisionRetentionPolicy(RevisionRetentionPolicyType revisionRetentionPolicyType)
        {
            _metadata.RevisionRetentionPolicyType = (int)revisionRetentionPolicyType;
        }

        public virtual TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            VerifyIsUnlocked();
            var document = TextDocument.Create(documentName, this.RevisionRetentionPolicy, creatorIdentity, initialData);
            _documents.Add(document);
            return document;
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

        public virtual void AddIdentity(Identity identity, AccessPermissionType accessPermissionType)
        {
            throw new NotImplementedException();
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
