using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestUtilities
{
    public static class TestConfig
    {
        public static CryptoManagerConfiguration Create(string userName)
        {
            CryptoManagerConfiguration configuration = new CryptoManagerConfiguration();
            configuration[ConfigurationKeyEnum.UserHome] = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\gnupg";
            configuration[ConfigurationKeyEnum.GPGBinaryPath] = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";
            return configuration;
        }

        public static CryptoManagerConfiguration GetTestConfiguration(string userName)
        {
            CryptoManagerConfiguration configuration = TestConfig.Create(userName);
            configuration[ConfigurationKeyEnum.UserHome] = TestUtils.GetUserDir(userName);
            return configuration;
        }
    }
}
