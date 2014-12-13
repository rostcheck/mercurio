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

        private Container(string name, RevisionRetentionPolicyType retentionPolicy)
        {
            Name = name;
            _retentionPolicy = RevisionRetentionPolicy.Create(retentionPolicy);
        }

        public static Container Create(string name, RevisionRetentionPolicyType retentionPolicy = RevisionRetentionPolicyType.KeepOne)
        {
            return new Container(name, retentionPolicy);
        }

        public string Name { get; private set; }

        public TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null)
        {
            return TextDocument.Create(documentName, _retentionPolicy, creatorIdentity, initialData);
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
