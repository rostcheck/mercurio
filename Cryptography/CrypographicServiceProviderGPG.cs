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
            var paths = Environment.ExpandEnvironmentVariables(Environment.GetEnvironmentVariable("PATH")).Split(osAbstractor.GetPathSeparatorChar());
            foreach (var path in paths)
            {
                var thisPath = Path.Combine(path, osAbstractor.GetExecutableName("gpg2"));
                if (File.Exists(thisPath))
                    return true;
            }
            return false;
        }

        public CryptoManagerConfiguration GetConfiguration()
        {
            var configuration = new CryptoManagerConfiguration();
            configuration.Add(CryptoConfigurationKeyEnum.CryptoExeBinaryPath.ToString(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GNU", "GnuPG", "gpg2.exe").ToString());
            configuration.Add(CryptoConfigurationKeyEnum.KeyringPath.ToString(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuPG").ToString());
            return configuration;
        }

        public ICryptoManager CreateManager(CryptoManagerConfiguration configuration)
        {
            return new GPGManager(configuration);
        }
    }
}
