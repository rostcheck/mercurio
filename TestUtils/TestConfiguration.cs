using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Mercurio.Domain;
using Cryptography.GPG;

namespace TestUtilities
{
    public static class TestConfig
    {
        public static CryptoManagerConfiguration Create(string userName)
        {
            CryptoManagerConfiguration configuration = new CryptoManagerConfiguration();
            configuration[GPGConfigurationKeyEnum.UserHome.ToString()] = UserHomeConfig();
            configuration[GPGConfigurationKeyEnum.GPGBinaryPath.ToString()] = BinaryPath();
            return configuration;
        }

        public static CryptoManagerConfiguration GetTestConfiguration(string userName)
        {
            CryptoManagerConfiguration configuration = TestConfig.Create(userName);
            configuration[GPGConfigurationKeyEnum.UserHome.ToString()] = TestUtils.GetUserDir(userName);
            return configuration;
        }

        private static string UserHomeConfig()
        {
            return Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "gnupg").ToString();
        }

        private static string BinaryPath()
        {
            return Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86), "GNU", "GnuPG", "gpg2.exe").ToString();
        }
    }
}
