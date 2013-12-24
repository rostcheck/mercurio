﻿using System;
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
        string ImportKey(string key); // returns imported key id
        void SignKey(string key);
        string[] GetSignatures();
        string GetFingerprint(string identifier);
        void DeleteKey(string identifier);
        Identity[] GetAvailableIdentities();
    }
}
