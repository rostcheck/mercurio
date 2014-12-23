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
    public class Container
    {
        private IRevisionRetentionPolicy _retentionPolicy;
        private List<IDocument> _documents;
        public List<IDocument> Documents 
        { 
            get
            {
                return new List<IDocument>(_documents);
            }
            private set
            {
                _documents = value;
            }
        }

        private Container(string name, RevisionRetentionPolicyType retentionPolicy)
        {
            Name = name;
            _retentionPolicy = RevisionRetentionPolicy.Create(retentionPolicy);
            _documents = new List<IDocument>();
        }

        public static Container Create(string name, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return new Container(name, retentionPolicy);
        }

        public string Name { get; private set; }

        public TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            var document = TextDocument.Create(documentName, _retentionPolicy, creatorIdentity, initialData);
            _documents.Add(document);
            return document;
        }

        public void DeleteRecord(string recordId)
        {
            throw new NotImplementedException();
        }

        public void ChangeRecord(Record changedRecord)
        {
            throw new NotImplementedException();
        }

        public void AddIdentity(Identity identity, AccessPermissionType accessPermissionType)
        {
            throw new NotImplementedException();
        }
    }
}
