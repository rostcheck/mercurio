using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Responsible for providing cryptographic functions (with a commmon interface)
    /// </summary>
    public interface ICryptoManager
    {
        void SetCredential(NetworkCredential credential);
        string GetActiveIdentity();
        void SetConfiguration(CryptoManagerConfiguration configuration);
        bool ValidateCredential(NetworkCredential credential);
        Stream Encrypt(Stream messageStream, string identifier);
        string Encrypt(string message, string identifier);
        Stream Decrypt(Stream encryptedMessageStream);
        string Decrypt(string encryptedMessage);
        string Sign(string message);
        bool Validate(string message);
        string GetPublicKey(string identifier); // for a single key
        string ImportKey(string key); // returns imported key id
        void SignKey(string identifier);
        string[] GetSignatures();
        string GetFingerprint(string identifier);
        void DeleteKey(string identifier);
        bool HasPublicKey(string key); // Public key exists in keychain
        List<UserIdentity> GetAvailableIdentities();
        List<ContactIdentity> GetAvailableContactIdentities(); // Other possible contacts to send to
        void CreateKey(string identifier, NetworkCredential credential);
        string ManagerType { get; } // string identifying the CryptoManager (ex. "GPG")
    }
}
