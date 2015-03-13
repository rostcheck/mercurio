using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Starksoft.Cryptography.OpenPGP;
using Entities;
using Mercurio.Domain;

namespace Cryptography.GPG
{
    public class GPGManager : ICryptoManager
    {
        public class KeyInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private CryptoManagerConfiguration configuration;
        private GnuPG gpg;
        private delegate void GpgOperation(Stream inputStream, Stream outputStream, Stream metadataStream);

        public string ManagerType
        {
            get
            {
                return "GPG";
            }
        }

        public GPGManager(CryptoManagerConfiguration configuration)
        {
            this.configuration = configuration;
             gpg = new GnuPG(configuration[GPGConfigurationKeyEnum.UserHome.ToString()],
                configuration[GPGConfigurationKeyEnum.GPGBinaryPath.ToString()]);
        }

        public void SetConfiguration(CryptoManagerConfiguration configuration)
        {
            this.configuration = configuration;
            gpg = new GnuPG(configuration[GPGConfigurationKeyEnum.UserHome.ToString()],
               configuration[GPGConfigurationKeyEnum.GPGBinaryPath.ToString()]);
        }

        public void SetCredential(NetworkCredential credential)
        {
            gpg.Credential = credential;
        }

        public bool ValidateCredential(NetworkCredential credential)
        {
            NetworkCredential savedCredential = gpg.Credential;
            gpg.Credential = credential;
            try
            {
                string result = ExecuteGPGStringOperation(gpg.Sign, "some nonsense message");
                gpg.Credential = savedCredential;
                return result != string.Empty;
            }
            catch (GnuPGException)
            {
                gpg.Credential = savedCredential;
                return false;
            }
        }

        public string GetActiveIdentity()
        {
            return (gpg.Credential == null) ? string.Empty : gpg.Credential.UserName;
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
            operation(messageStream, resultStream, metadataStream);
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

        public string GetPublicKey(string requestedIdentity)
        {
            var identity = requestedIdentity;
            if (identity == null || identity == "")
            {
                var firstSecretKey = gpg.GetSecretKeys().FirstOrDefault<GnuPGKey>();
                if (firstSecretKey != null)
                    identity = firstSecretKey.KeyID;
                else
                    throw new MercurioException("No default public key is available");
            }
            GnuPGKey publicKey = gpg.GetKeys().FirstOrDefault(s => s.KeyID == identity);
            if (publicKey == null && requestedIdentity != null && requestedIdentity != "")
            {
                throw new MercurioException(string.Format("Requested public key {0} is not available", requestedIdentity));
            }

            return (publicKey == null) ? "" : gpg.GetActualKey(publicKey.KeyID);
        }

        public string ImportKey(string key)
        {
            return gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(key)));
        }

        public string[] GetSignatures()
        {
            //throw new NotImplementedException();
            return new string[0];
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

        public List<UserIdentity> GetAvailableIdentities()
        {
            List<UserIdentity> userIdentityList = new List<UserIdentity>();
            GnuPGKeyCollection secretKeys = gpg.GetSecretKeys();
            foreach (GnuPGKey secretKey in secretKeys)
            {
                var keyInfo = GetKeyInfo(secretKey);
                userIdentityList.Add(UserIdentity.Create(secretKey.KeyID, keyInfo.Name, secretKey.UserId, keyInfo.Description, this.ManagerType));
            }
            return userIdentityList;
        }

        public List<ContactIdentity> GetAvailableContactIdentities()
        {
            List<ContactIdentity> contactIdentityList = new List<ContactIdentity>();
            GnuPGKeyCollection publicKeys = gpg.GetKeys();
            foreach (GnuPGKey key in publicKeys)
            {
                var keyInfo = GetKeyInfo(key);
                contactIdentityList.Add(ContactIdentity.Create(key.KeyID, keyInfo.Name, key.UserId, keyInfo.Description, this.ManagerType));
            }
            return contactIdentityList;
        }

        private KeyInfo GetKeyInfo(GnuPGKey key)
        {
            var keyInfo = new KeyInfo();

            // Match expressions of form: Alice (Alice's Key) 
            Match match = Regex.Match(key.UserName, @"(.+)\((.+)\)", RegexOptions.None);
            if (match.Success)
            {
                keyInfo.Name = match.Groups[1].Value;
                keyInfo.Description = match.Groups[2].Value;
            }
            else
            {
                keyInfo.Name = key.UserName;
            }
            match = Regex.Match(keyInfo.Name, @"\[(.+)\]\s+(.+)", RegexOptions.None);
            if (match.Success)
            {
                keyInfo.Name = match.Groups[2].Value;
            }
            return keyInfo;
        }

        public void CreateKey(string identifier, NetworkCredential credential)
        {
            throw new NotImplementedException();
        }
    }
}
