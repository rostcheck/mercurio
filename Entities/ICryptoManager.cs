using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    // Responsible for providing cryptographic functions (with a commmon interface)
    public interface ICryptoManager
    {
        void SetPassphrase(string passphrase);
        Stream Encrypt(Stream messageStream, EncryptionAlgorithmEnum algorithm);
        string Encrypt(string message, EncryptionAlgorithmEnum algorithm);
        string Sign(string message);
        bool Validate(string message);
        string GetPublicKey(string identifier); // for a single key
        string ImportKey(string key); // returns imported key id
        void SignKey(string key);
        string[] GetSignatures();
        string GetFingerprint(string identifier);
        void DeleteKey(string identifier);
        bool HasPublicKey(string key); // Public key exists in keychain
        List<User> GetAvailableIdentities();
        List<User> GetAvailableUsers(); // Other possible users to send to
    }
}
