using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;
using System.Configuration;
using System.Reflection;
using Cryptography.GPG;

namespace Mercurio.Domain.Implementation
{
    /// <summary>
    /// Scans the operating environment to find the available cryptographic providers, storage substrates, and similar resources
    /// </summary>
    public class EnvironmentScanner : IEnvironmentScanner
    {
        private List<ICryptographicServiceProvider> providerList;

        public List<ICryptographicServiceProvider> GetCryptographicProviders()
        {
            providerList = new List<ICryptographicServiceProvider>();

            // List of crypographic providers the scanner knows how to find
            RegisterCryptographicProvider(typeof(CrypographicServiceProviderGPG));
            return providerList;
        }

        private void RegisterCryptographicProvider(Type type)
        {
            CryptoManagerFactory.Register(type.Name, type);
            providerList.Add((ICryptographicServiceProvider)Activator.CreateInstance(type));
        }

        public List<IStorageSubstrate> GetStorageSubstrates()
        {
            var returnList = new List<IStorageSubstrate>();
            if (ConfigurationManager.AppSettings["StorageSubstrate"] != null)
            {
                returnList.Add(DiskStorageSubstrate.Create(ConfigurationManager.AppSettings["StorageSubstrate"], SerializerType.BinarySerializer));
            }
            return returnList;
        }
    }
}
