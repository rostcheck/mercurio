using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starksoft.Cryptography.OpenPGP;

namespace Entities
{
    public class GPGManager : ICryptoManager
    {
        private Dictionary<ConfigurationKeyEnum, string> configuration;
        private GnuPG gpg;

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

        public string Encrypt(string message, EncryptionAlgorithmEnum algorithm)
        {
            throw new NotImplementedException();
        }

        public string Sign(string message)
        {
            throw new NotImplementedException();
        }

        public bool Validate(string message)
        {
            throw new NotImplementedException();
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

        public Identity[] GetAvailableIdentities()
        {
            List<Identity> identityList = new List<Identity>();
            GnuPGKeyCollection secretKeys = gpg.GetSecretKeys();
            foreach (GnuPGKey secretKey in secretKeys)
            {
                identityList.Add(new Identity(secretKey.KeyID, secretKey.UserName));
            }
            return identityList.ToArray();
        }
    }
}
