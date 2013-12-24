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

        public GPGManager(Dictionary<ConfigurationKeyEnum, string> configuration)
        {
            this.configuration = configuration;
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
            GnuPG gpg = new GnuPG(configuration[ConfigurationKeyEnum.UserHome],
                configuration[ConfigurationKeyEnum.GPGBinaryPath]);
            GnuPGKey firstSecretKey = gpg.GetSecretKeys().FirstOrDefault<GnuPGKey>();
            GnuPGKey publicKey = gpg.GetKeys().FirstOrDefault(s => s.KeyID == firstSecretKey.KeyID);
            return gpg.GetActualKey(publicKey.KeyID);
        }

        public string ImportKey(string key)
        {
            GnuPG gpg = new GnuPG(configuration[ConfigurationKeyEnum.UserHome],
                configuration[ConfigurationKeyEnum.GPGBinaryPath]);
            return gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(key)));
        }

        public string[] GetSignatures()
        {
            throw new NotImplementedException();
        }

        public string GetFingerprint(string identity)
        {
            GnuPG gpg = new GnuPG(configuration[ConfigurationKeyEnum.UserHome],
                configuration[ConfigurationKeyEnum.GPGBinaryPath]);
            return gpg.GetFingerprint(identity);
        }

        public void DeleteKey(string identity)
        {
            GnuPG gpg = new GnuPG(configuration[ConfigurationKeyEnum.UserHome],
                configuration[ConfigurationKeyEnum.GPGBinaryPath]);
            gpg.DeleteKey(identity);
        }
    }
}
