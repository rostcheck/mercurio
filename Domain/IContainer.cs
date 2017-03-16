using System;
using System.Collections.Generic;
using System.IO;

namespace Mercurio.Domain
{
    /// <summary>
    /// A collection of Documents
    /// </summary>
    public interface IContainer
    {
        // Basics
        Guid Id { get; }
        string Name { get; }
        string CryptoManagerType { get; }
		string RevisionRetentionPolicyType { get; } // must be unlocked to get
        IRevisionRetentionPolicy RevisionRetentionPolicy { get; }

        // Container manipulation
        bool IsLocked { get; }
        void Lock(ICryptoManager cryptoManager);
        void Unlock(byte[] privateMetadataBytes, ICryptoManager cryptoManager);
        bool IsAvailableToIdentity(string uniqueIdentifier);
        void AddIdentity(Identity identity, AccessPermissionType accessPermissionType);
        void ChangeRevisionRetentionPolicy(RevisionRetentionPolicyType revisionRetentionPolicyType);

        // Content enumeration
        ICollection<string> Documents { get; }
        ICollection<string> AllDocuments { get; }
        ICollection<string> DeletedDocuments { get; }
        ICollection<DocumentVersionMetadata> ListAvailableVersions(string documentName);
        DocumentVersion GetDocumentVersion(DocumentVersionMetadata versionMetadata, bool metadataOnly = false);
        DocumentVersion GetLatestDocumentVersion(string documentName);
        DocumentVersionMetadata GetLatestDocumentVersionMetadata(string documentName);
        bool ContainsDocument(string documentName);

        // Database manipulation
        void CreateDatabase(string databaseName, Identity creatorIdentity);
        void AttachDatabaseSchema(string databaseName, DatabaseSchema schema, Identity modifierIdentity);
        void AddDatabaseRecord(string databaseName, Record record, Identity modifierIdentity);
        void AddDatabaseRecords(string databaseName, IEnumerable<Record> records, Identity modifierIdentity);
        void ChangeDatabaseRecord(string databaseName, Record record, Identity modifierIdentity);
        void DeleteDatabaseRecord(string databaseName, Guid recordId, Identity modifierIdentity);
        List<Record> GetDatabaseRecords(string databaseName);
        List<Record> GetRecordsWhere(string databaseName, Func<Record, bool> predicate);

        // Document manipulation
        DocumentVersion CreateDocument(string documentName, DocumentType documentType, Identity creatorIdentity, Stream dataStream);
        DocumentVersion CreateTextDocument(string documentName, Identity creatorIdentity, string initialData);
        DocumentVersion ModifyTextDocument(string documentName, Identity modifierIdentity, string modifiedData);
        DocumentVersionMetadata DeleteDocumentSoft(string documentName, Identity modifierIdentity);
        void DeleteDocumentHard(string documentName, Identity modifierIdentity);
        DocumentVersion UnDeleteDocument(string documentName, Identity modifierIdentity); // Can only undelete soft-deleted documents
        void RenameDocument(string oldDocumentName, string newDocumentName);
    }
}
