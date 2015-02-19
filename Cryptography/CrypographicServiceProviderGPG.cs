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

        public bool IsInstalled()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GNU", "GnuPG", "gpg2.exe");
            return File.Exists(path);
        }

        public CryptoManagerConfiguration GetConfiguration()
        {
            var configuration = new CryptoManagerConfiguration();
            configuration.Add(GPGConfigurationKeyEnum.GPGBinaryPath.ToString(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GNU", "GnuPG", "gpg2.exe").ToString());
            configuration.Add(GPGConfigurationKeyEnum.UserHome.ToString(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GnuPG").ToString());
            return configuration;
        }

        public ICryptoManager CreateManager(CryptoManagerConfiguration configuration)
        {
            return new GPGManager(configuration);
        }
    }
}
