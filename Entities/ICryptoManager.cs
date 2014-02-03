﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    // Responsible for providing cryptographic functions (with a commmon interface)
    public interface ICryptoManager
    {
        void SetCredential(NetworkCredential credential);
        Stream Encrypt(Stream messageStream, string identifier);
        string Encrypt(string message, string identifier);
        string EncryptSymmetric(string message, EncryptionAlgorithmEnum algorithm);

        Stream Decrypt(Stream messageStream);
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
