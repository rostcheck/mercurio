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
    public interface IContainer
    {
        string Id { get; }
        string Name { get; }
        string CryptoManagerType { get; }
        bool IsLocked { get; }
        void Lock();
        void Unlock(ICryptoManager cryptoManager);
        bool IsAvailableToIdentity(string uniqueIdentifier);
        void AddIdentity(Identity identity, AccessPermissionType accessPermissionType);
        IRevisionRetentionPolicy RevisionRetentionPolicy { get;  }
        void ChangeRevisionRetentionPolicy(RevisionRetentionPolicyType revisionRetentionPolicyType);
        ICollection<string> Documents { get; }
        ICollection<DocumentVersionMetadata> GetAvailableVersions(string documentName);
        DocumentVersionMetadata GetLatestDocumentVersion(string documentName);

        DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData);
        DocumentVersion ModifyTextDocument(string documentName, Identity modifierIdentity, string modifiedData);
        DocumentVersion RetrieveDocument(string documentName); // latest version
        DocumentVersion RetrieveDocument(string documentName, Guid specificVersionId);
        void DeleteRecord(string recordId);
        void ChangeRecord(Record changedRecord);
    }
}
