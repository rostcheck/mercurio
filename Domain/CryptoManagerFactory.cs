using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Registers available CryptoManagers on the system and creates them on command
    /// </summary>
    public static class CryptoManagerFactory
    {
        private static Dictionary<string, Type> registry;
        private static Dictionary<string, CryptoManagerConfiguration> configurationRegistry;

        public static ICryptoManager Create(string cryptoManagerType, CryptoManagerConfiguration userConfiguration = null)
        {
            if (registry == null)
            {
                throw new MercurioException("No CryptoManager types are registered with CryptoManagerFactory");
            }

            if (configurationRegistry == null)
            {
                configurationRegistry = new Dictionary<string, CryptoManagerConfiguration>();
            }

            if (registry.ContainsKey(cryptoManagerType.ToLower()))
            {
                var cryptoServiceProviderType = registry[cryptoManagerType.ToLower()];
                var cryptoServiceProvider = Activator.CreateInstance(cryptoServiceProviderType) as ICryptographicServiceProvider;
                CryptoManagerConfiguration providerConfiguration;
                if (configurationRegistry.ContainsKey(cryptoManagerType.ToLower()))
                {
                    providerConfiguration = configurationRegistry[cryptoManagerType.ToLower()];
                }
                else
                {
                    providerConfiguration = cryptoServiceProvider.GetConfiguration();
                }

                providerConfiguration.Merge(userConfiguration);
                return cryptoServiceProvider.CreateManager(providerConfiguration);
            }
            else
            {
                throw new MercurioException(string.Format("CryptoManager type {0} is not available on this system", cryptoManagerType));
            }
        }

        // Can supply optional configuration primarily for testing (force provider to use a specific configuration)
        public static void Register(string cryptoManagerName, Type cryptoManagerType, CryptoManagerConfiguration configuration = null)
        {
            if (cryptoManagerType.FindInterfaces(InterfaceNameFilter, typeof(ICryptographicServiceProvider).Name).Length == 0)
            {
                throw new ArgumentException("Type must support interface ICryptographicServiceProvider");
            }

            if (registry == null)
            {
                registry = new Dictionary<string, Type>();
            }
            if (!registry.ContainsKey(cryptoManagerName.ToLower()))
            {
                registry.Add(cryptoManagerName.ToLower(), cryptoManagerType);
            }
            if (configuration != null)
            {
                if (configurationRegistry == null)
                {
                    configurationRegistry = new Dictionary<string, CryptoManagerConfiguration>();
                }
                configurationRegistry[cryptoManagerName.ToLower()] = configuration;
            }
        }

        private static bool InterfaceNameFilter(Type typeObj, Object criteriaObj)
        {
            return (typeObj.Name == criteriaObj.ToString());
        }
    }
}
