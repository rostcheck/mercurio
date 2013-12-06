using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace TestEntities
{
    public static class TestConfiguration
    {
        public static Dictionary<ConfigurationKeyEnum, string> Create()
        {
            Dictionary<ConfigurationKeyEnum, string> configuration = new Dictionary<ConfigurationKeyEnum, string>();
            configuration[ConfigurationKeyEnum.UserHome] = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\gnupg";
            configuration[ConfigurationKeyEnum.GPGBinaryPath] = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";
            return configuration;
        }
    }
}
