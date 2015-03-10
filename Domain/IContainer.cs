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
        List<IDocument> Documents { get; }
        bool IsLocked { get; }
        void Lock();
        void Unlock(ICryptoManager cryptoManager);

        string Name { get; }
        string CryptoManagerType { get; }

        TextDocument CreateTextDocument(string documentName, Identity creatorIdentity, string initialData = null);
        void DeleteRecord(string recordId);
        void ChangeRecord(Record changedRecord);
        void AddIdentity(Identity identity, AccessPermissionType accessPermissionType);
        bool IsAvailableToIdentity(string uniqueIdentifier);
    }
}
