﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A collection of Documents
    /// </summary>
    public interface IContainer
    {
        Guid Id { get; }
        string Name { get; }
        string CryptoManagerType { get; }
		string RevisionRetentionPolicyType { get; } // must be unlocked to get
        bool IsLocked { get; }
        void Lock(ICryptoManager cryptoManager);
        void Unlock(byte[] privateMetadataBytes, ICryptoManager cryptoManager);
        bool IsAvailableToIdentity(string uniqueIdentifier);
        void AddIdentity(Identity identity, AccessPermissionType accessPermissionType);
        IRevisionRetentionPolicy RevisionRetentionPolicy { get;  }
        void ChangeRevisionRetentionPolicy(RevisionRetentionPolicyType revisionRetentionPolicyType);
        ICollection<string> Documents { get; }
        ICollection<DocumentVersionMetadata> ListAvailableVersions(string documentName);
        DocumentVersion GetDocumentVersion(DocumentVersionMetadata versionMetadata, bool metadataOnly = false);
        DocumentVersion GetLatestDocumentVersion(string documentName, bool metadataOnly = false);

        DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData);
        DocumentVersion ModifyTextDocument(string documentName, Identity modifierIdentity, string modifiedData);
        void DeleteRecord(string recordId);
        void ChangeRecord(Record changedRecord);

        bool ContainsDocument(string documentName);
        DocumentVersion DeleteDocumentSoft(string documentName, Identity modifierIdentity);
        void DeleteDocumentHard(string documentName, Identity modifierIdentity);
        DocumentVersion UnDeleteDocument(string documentName, Identity modifierIdentity); // Can only undelete soft-deleted documents
        void RenameDocument(string oldDocumentName, string newDocumentName);
    }
}
