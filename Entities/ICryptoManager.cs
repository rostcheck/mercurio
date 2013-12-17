using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    // Responsible for providing cryptographic functions (with a commmon interface)
    public interface ICryptoManager
    {
        string Encrypt(string message, EncryptionAlgorithmEnum algorithm);
        string Sign(string message);
        bool Validate(string message);
        string GetPublicKey(string identifier);
        void ImportKey(string key);
        string[] GetSignatures();
    }
}
