using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;
using System.Reflection;
using Cryptography.GPG;
using System.IO;

namespace Mercurio.Domain.Implementation
{
    /// <summary>
    /// Scans the operating environment to find the available cryptographic providers, storage substrates, and similar resources
    /// </summary>
    public class EnvironmentScanner : IEnvironmentScanner
    {
        private List<ICryptographicServiceProvider> possibleCryptoProviderList;
        private ITempStorageSubstrate _tempDiskStorageSubstrate;

        public EnvironmentScanner(string userHome = null)
        {
            CryptoManagerConfiguration configuration = null;
            if (userHome != null)
            {
                configuration = new CryptoManagerConfiguration();
                configuration.Add(CryptoConfigurationKeyEnum.KeyringPath.ToString(), userHome);
            }
            possibleCryptoProviderList = new List<ICryptographicServiceProvider>();
            // Manually maintained list of things we know how to find
            RegisterCryptographicProvider(typeof(CrypographicServiceProviderGPG), configuration);
        }

        public List<ICryptographicServiceProvider> GetCryptographicProviders()
        {
            var availableProviders = new List<ICryptographicServiceProvider>();

            foreach (var provider in possibleCryptoProviderList)
            {
                if (provider.IsInstalled())
                {
                    availableProviders.Add(provider);
                }
            }
            return availableProviders;
        }

        private void RegisterCryptographicProvider(Type type, CryptoManagerConfiguration configuration = null)
        {
            CryptoManagerFactory.Register(type.Name, type, configuration);
            possibleCryptoProviderList.Add((ICryptographicServiceProvider)Activator.CreateInstance(type));
        }

        public List<IStorageSubstrate> GetStorageSubstrates()
        {
            var returnList = new List<IStorageSubstrate>();
            var storageSubstratePath = ConfigurationManager.GetConfigurationValue("StorageSubstrate");
            if (storageSubstratePath != null)
            {
                returnList.Add(DiskStorageSubstrate.Create(storageSubstratePath, SerializerType.BinarySerializer));
            }
            return returnList;
        }

        public ITempStorageSubstrate GetTemporaryStorageSubstrate()
        {
            if (_tempDiskStorageSubstrate == null)
            {
                var tempStorageSubstratePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Guid.NewGuid().ToString()).ToString();
                Directory.CreateDirectory(tempStorageSubstratePath);
                _tempDiskStorageSubstrate = DiskTempStorageSubstrate.Create(tempStorageSubstratePath);
            }
            return _tempDiskStorageSubstrate;
        }

        public string GetEditor()
        {
            // hard-coded to Notepad for now
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows).ToString(), "Notepad.exe").ToString(); 
        }
    }
}
