using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;
using System.IO;

namespace Cryptography.GPG
{
    public class CrypographicServiceProviderGPG : ICryptographicServiceProvider
    {
        public string GetProviderType()
        {
            return CryptoType.GPG.ToString();
        }

        public bool IsInstalled(IOSAbstractor osAbstractor)
        {
            return !string.IsNullOrEmpty(GetInstalledGPGPath(osAbstractor));
        }

        private string GetInstalledGPGPath(IOSAbstractor osAbstractor)
        {
            var paths = Environment.ExpandEnvironmentVariables(Environment.GetEnvironmentVariable("PATH")).Split(osAbstractor.GetPathSeparatorChar());
            foreach (var path in paths)
            {
                var thisPath = Path.Combine(path, osAbstractor.GetExecutableName("gpg2"));
                if (File.Exists(thisPath))
                    return thisPath;
            }
            return null;
        }

        public CryptoManagerConfiguration GetConfiguration(IOSAbstractor osAbstractor)
        {
            var configuration = new CryptoManagerConfiguration();
            configuration.Add(CryptoConfigurationKeyEnum.CryptoExeBinaryPath.ToString(), GetInstalledGPGPath(osAbstractor));
            configuration.Add(CryptoConfigurationKeyEnum.KeyringPath.ToString(), GetKeyringPath(osAbstractor));
            return configuration;
        }

        private string GetKeyringPath(IOSAbstractor osAbstractor)
        {
            if (osAbstractor.GetOsType() == OSType.Windows)
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuPG").ToString();
            else
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".gnupg");
        }

        public ICryptoManager CreateManager(CryptoManagerConfiguration configuration)
        {
            return new GPGManager(configuration);
        }
    }
}
