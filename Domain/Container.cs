using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A collection of Records
    /// </summary>
    public class Container : IContainer
    {
        private IRevisionRetentionPolicy _retentionPolicy;
        private IStoragePlan _storagePlan;
        private List<IDocument> _documents;
        private bool _locked;

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

        protected Container(string name, IStoragePlan storagePlan, RevisionRetentionPolicyType retentionPolicy)
        {
            Name = name;
            _retentionPolicy = RevisionRetentionPolicy.Create(retentionPolicy);
            _storagePlan = storagePlan;
            _documents = new List<IDocument>();
            _locked = false;
        }

        protected Container()
        {
        }

        // Container is created unlocked
        public static Container Create(string name, IStoragePlan storagePlan, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return new Container(name, storagePlan, retentionPolicy);
        }

        public bool IsLocked 
        { 
            get 
            { 
                return _locked; 
            } 
        }

        public void Lock()
        {
            _locked = true;
        }

        public string Name { get; private set; }

        public virtual TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            VerifyIsUnlocked();
            var document = TextDocument.Create(documentName, _retentionPolicy, creatorIdentity, initialData);
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
            if (_locked)
            {
                throw new UnauthorizedAccessException("Container is locked");
            }
        }
    }
}
