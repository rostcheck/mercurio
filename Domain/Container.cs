using System;
using System.Collections.Generic;
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

        protected Container(string name, RevisionRetentionPolicyType retentionPolicy)
        {
            Name = name;
            this.RevisionRetentionPolicy = Mercurio.Domain.RevisionRetentionPolicy.Create(retentionPolicy);
            _documents = new List<IDocument>();
            _locked = false;
        }

        //protected Container()
        //{
        //}

        protected virtual IRevisionRetentionPolicy RevisionRetentionPolicy { get; private set; }
       
        // Container is created unlocked
        public static Container Create(string name, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return new Container(name, retentionPolicy);
        }

        public bool IsLocked 
        { 
            get 
            { 
                return _locked; 
            } 
        }

        public virtual void Lock()
        {
            _locked = true;
        }

        public virtual void Unlock()
        {
            _locked = false;
        }

        public virtual string Name { get; protected set; }

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
            if (_locked)
            {
                throw new UnauthorizedAccessException("Container is locked");
            }
        }
    }
}
