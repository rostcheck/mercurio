using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace MercurioAppServiceLayer
{
    public interface IMessageStore
    {
        bool IsLocked { get; }
        AppCryptoManagerType CryptoType { get; }
        bool Unlock(ICryptoManager cryptoManager, string passphrase);
        void Lock(ICryptoManager cryptoManager, string passphrase);
        List<IMercurioMessage> GetMessages(string identifier);
        // Returns true if message was replaced
        bool Store(IMercurioMessage message, string identifier);
    }
}
