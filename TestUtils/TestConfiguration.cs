using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestUtils
{
    public static class TestConfiguration1
    {

        public static Dictionary<ConfigurationKeyEnum, string> Create()
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = new Dictionary<ConfigurationKeyEnum, string>();
            configuration[ConfigurationKeyEnum.UserHome] = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\gnupg";
            configuration[ConfigurationKeyEnum.GPGBinaryPath] = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";
            return configuration;
        }

        public static Dictionary<ConfigurationKeyEnum, string> GetTestConfiguration(string userName)
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = TestConfiguration1.Create();
            configuration[ConfigurationKeyEnum.UserHome] = TestUtils.GetUserDir(userName);
            return configuration;
        }
    }
}
