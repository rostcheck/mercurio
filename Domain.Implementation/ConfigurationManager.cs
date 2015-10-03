using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    // Provides configuration management with some keyword expansion
    public class ConfigurationManager
    {
        public static string GetConfigurationValue(string configurationValueName)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[configurationValueName] != null)
            {
                var configurationValue = System.Configuration.ConfigurationManager.AppSettings[configurationValueName];
                return ReplaceConfigurationStrings(configurationValue);
            }
            return null;
        }

        private static string ReplaceConfigurationStrings(string inputString)
        {
            if (inputString.Contains("{DocumentDirectory}"))
                return inputString.Replace("{DocumentDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar);
            return inputString;
        }
    }
}
