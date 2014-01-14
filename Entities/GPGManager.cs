﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Starksoft.Cryptography.OpenPGP;

namespace Entities
{
    public class GPGManager : ICryptoManager
    {
        private Dictionary<ConfigurationKeyEnum, string> configuration;
        private GnuPG gpg;
        private delegate void GpgOperation(Stream inputStream, Stream outputStream, Stream metadataStream);

        public GPGManager(Dictionary<ConfigurationKeyEnum, string> configuration)
        {
            this.configuration = configuration;
             gpg = new GnuPG(configuration[ConfigurationKeyEnum.UserHome],
                configuration[ConfigurationKeyEnum.GPGBinaryPath]);
        }

        //TODO: replace w/ SecureString
        public void SetPassphrase(string passphrase)
        {
            gpg.Passphrase = passphrase;
        }

        private Stream ExecuteGPGStreamOperation(GpgOperation operation, Stream messageStream)
        {
            MemoryStream outputStream = new MemoryStream();
            MemoryStream metadataStream = new MemoryStream();
            operation(messageStream, outputStream, metadataStream);
            return outputStream;
        }

        private string ExecuteGPGStringOperation(GpgOperation operation, string message)
        {
            MemoryStream messageStream = new MemoryStream(Encoding.ASCII.GetBytes(message));
            MemoryStream resultStream = new MemoryStream();
            MemoryStream metadataStream = new MemoryStream();
            gpg.Encrypt(messageStream, resultStream, metadataStream);
            resultStream.Position = 0;
            StreamReader reader = new StreamReader(resultStream);
            return reader.ReadToEnd();
        }

        public Stream Encrypt(Stream messageStream, string identifier)
        {
            gpg.Recipient = identifier;
            return ExecuteGPGStreamOperation(gpg.Encrypt, messageStream);
        }

        public string Encrypt(string message, string identifier)
        {
            gpg.Recipient = identifier;
            return ExecuteGPGStringOperation(gpg.Encrypt, message);
        }

        public Stream Decrypt(Stream messageStream)
        {
            return ExecuteGPGStreamOperation(gpg.Decrypt, messageStream);
        }

        public string Decrypt(string message)
        {
            return ExecuteGPGStringOperation(gpg.Decrypt, message);
        }

        public string EncryptSymmetric(string message, EncryptionAlgorithmEnum algorithm)
        {
            throw new NotImplementedException();
        }

        public bool Validate(string message)
        {
            throw new NotImplementedException();
        }

        public Stream Sign(Stream messageStream)
        {
            return ExecuteGPGStreamOperation(gpg.Sign, messageStream);
        }

        public string Sign(string message)
        {
            return ExecuteGPGStringOperation(gpg.Sign, message);
        }

        public string GetPublicKey(string identity)
        {
            GnuPGKey firstSecretKey = gpg.GetSecretKeys().FirstOrDefault<GnuPGKey>();
            GnuPGKey publicKey = gpg.GetKeys().FirstOrDefault(s => s.KeyID == firstSecretKey.KeyID);
            return gpg.GetActualKey(publicKey.KeyID);
        }

        public string ImportKey(string key)
        {
            return gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(key)));
        }

        public string[] GetSignatures()
        {
            throw new NotImplementedException();
        }

        public string GetFingerprint(string identity)
        {
            return gpg.GetFingerprint(identity);
        }

        public void SignKey(string identity)
        {
            gpg.SignKey(identity);
        }

        public void DeleteKey(string identity)
        {
            gpg.DeleteKey(identity);
        }

        public bool HasPublicKey(string key)
        {
            return (gpg.GetKeys().FirstOrDefault(s => s.KeyID == key) != null);
        }

        public List<User> GetAvailableIdentities()
        {
            List<User> identityList = new List<User>();
            GnuPGKeyCollection secretKeys = gpg.GetSecretKeys();
            foreach (GnuPGKey secretKey in secretKeys)
            {
                identityList.Add(MakeUser(secretKey));
            }
            return identityList;
        }

        public List<User> GetAvailableUsers()
        {
            List<User> userList = new List<User>();
            GnuPGKeyCollection publicKeys = gpg.GetKeys();
            foreach (GnuPGKey key in publicKeys)
            {
                userList.Add(MakeUser(key));
            }
            return userList;
        }

        private User MakeUser(GnuPGKey key)
        {
            string name = string.Empty;
            string description = string.Empty;
            // Match expressions of form: Alice (Alice's Key) 
            Match match = Regex.Match(key.UserName, @"(.+)\((.+)\)", RegexOptions.None);
            if (match.Success)
            {
                name = match.Groups[1].Value;
                description = match.Groups[2].Value;
            }
            else
            {
                name = key.UserName;
            }

            return new User(key.KeyID, name, key.UserId, description);
        }
    }
}
