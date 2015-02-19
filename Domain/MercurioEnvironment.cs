using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents an operating environment in which you can do persistent Mercurio storage operations
    /// </summary>
    public class MercurioEnvironment
    {
        private List<ICryptographicServiceProvider> _cryptographicServiceProviders;
        private List<IStorageSubstrate> _storageSubstrates;

        public static MercurioEnvironment Create(IEnvironmentScanner scanner)
        {
            var cryptographicServiceProviders = scanner.GetCryptographicProviders();
            var storageSubstrates = scanner.GetStorageSubstrates();

            if (cryptographicServiceProviders == null || storageSubstrates == null)
                throw new ArgumentNullException();

            if (!cryptographicServiceProviders.Any())
                throw new ArgumentException("Must provide at least one cryptographic service provider");

            if (!storageSubstrates.Any())
                throw new ArgumentException("Must provide at least one storage substrate");
            
            return new MercurioEnvironment(cryptographicServiceProviders, storageSubstrates);
        }

        private MercurioEnvironment(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, 
            IEnumerable<IStorageSubstrate> storageSubstrates)
        {
            this._cryptographicServiceProviders = new List<ICryptographicServiceProvider>(cryptographicServiceProviders);
            this._storageSubstrates = new List<IStorageSubstrate>(storageSubstrates);
        }

        public List<IContainer> GetContainers()
        {
            var returnList = new List<IContainer>();
            foreach(var substrate in this._storageSubstrates)
            {
                returnList.AddRange(substrate.GetContainers(_cryptographicServiceProviders));
            }
            return returnList;
        }

        public List<string> GetAvailableStorageSubstrateNames()
        {
            return new List<string>(_storageSubstrates.Select(s => s.Name));
        }
       
        public IContainer CreateContainer(string containerName, string storageSubstrateName,
            RevisionRetentionPolicyType revisionRetentionPolicyType = RevisionRetentionPolicyType.KeepOne, string cryptoProviderType = null)
        {
            var substrate = _storageSubstrates.SingleOrDefault(s => s.Name.ToLower() == storageSubstrateName.ToLower());
            if (substrate == null)
            {
                throw new ArgumentException(string.Format("Invalid storage substrate name {0}", storageSubstrateName));
            }
            ICryptographicServiceProvider cryptoProvider = null;
            if (cryptoProviderType == null || cryptoProviderType == "")
            {
                cryptoProvider = _cryptographicServiceProviders.FirstOrDefault();
                if (cryptoProvider == null)
                {
                    throw new MercurioExceptionRequiredCryptoProviderNotAvailable("No crypto providers are available on this system");
                }
            }
            else
            {
                cryptoProvider = _cryptographicServiceProviders.Where(s => s.GetProviderType() == cryptoProviderType).FirstOrDefault();
                if (cryptoProvider == null)
                {
                    throw new MercurioExceptionRequiredCryptoProviderNotAvailable(string.Format("Requested crypto provider {0} is not available on this system", cryptoProviderType));
                }
            }
            return substrate.CreateContainer(containerName, cryptoProvider, revisionRetentionPolicyType);
        }

        public IContainer GetContainer(string newContainerName)
        {
            return GetContainers().Where(s => s.Name == newContainerName).FirstOrDefault();
        }

        public List<UserIdentity> GetAvailableIdentities()
        {
            var identities = new List<UserIdentity>();
            foreach (var cryptographicStorageProvider in _cryptographicServiceProviders)
            {
                var manager = cryptographicStorageProvider.CreateManager(cryptographicStorageProvider.GetConfiguration());
                identities.AddRange(manager.GetAvailableIdentities());
            }
            return identities;
        }
    }
}
