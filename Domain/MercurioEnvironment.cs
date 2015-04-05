using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private UserIdentity _activeIdentity;
        private ICryptoManager _activeCryptoManager;
        private Func<string, NetworkCredential> _passphraseFunction;
        private NetworkCredential _activeCredential;
        private string _userHomeDirectory = null;
        private Serializer _serializer;

        public static MercurioEnvironment Create(IEnvironmentScanner scanner, Serializer serializer, Func<string, NetworkCredential> passphraseFunction)
        {
            var cryptographicServiceProviders = scanner.GetCryptographicProviders();
            var storageSubstrates = scanner.GetStorageSubstrates();

            if (cryptographicServiceProviders == null || storageSubstrates == null)
                throw new ArgumentNullException();

            if (!cryptographicServiceProviders.Any())
                throw new ArgumentException("Must provide at least one cryptographic service provider");

            if (!storageSubstrates.Any())
                throw new ArgumentException("Must provide at least one storage substrate");
            
            if (passphraseFunction == null)
            {
                throw new ArgumentNullException("Must provide a valid passphrase function");
            }
            return new MercurioEnvironment(cryptographicServiceProviders, serializer, storageSubstrates, passphraseFunction);
        }

        private MercurioEnvironment(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, 
            Serializer serializer,
            IEnumerable<IStorageSubstrate> storageSubstrates, Func<string, NetworkCredential> passphraseFunction)
        {
            this._cryptographicServiceProviders = new List<ICryptographicServiceProvider>(cryptographicServiceProviders);
            this._storageSubstrates = new List<IStorageSubstrate>(storageSubstrates);
            this._passphraseFunction = passphraseFunction;
            this._serializer = serializer;
        }

        // Generally only needed for testing
        public void SetUserHomeDirectory(string userHomeDirectory)
        {
            _userHomeDirectory = userHomeDirectory;
        }

        public List<IContainer> GetContainers()
        {
            VerifyActiveIdentity();

            var returnList = new List<IContainer>();
            foreach(var substrate in this._storageSubstrates)
            {
                returnList.AddRange(Container.GetAccessibleContainers(substrate, _activeIdentity.UniqueIdentifier, _activeCryptoManager.ManagerType));
            }
            return returnList;
        }

        public List<string> GetAvailableStorageSubstrateNames()
        {
            return new List<string>(_storageSubstrates.Select(s => s.Name));
        }
       
        public IContainer CreateContainer(string containerName, string storageSubstrateName,
            RevisionRetentionPolicyType revisionRetentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            VerifyActiveIdentity();

            var substrate = _storageSubstrates.SingleOrDefault(s => s.Name.ToLower() == storageSubstrateName.ToLower());
            if (substrate == null)
            {
                throw new ArgumentException(string.Format("Invalid storage substrate name {0}", storageSubstrateName));
            }
            return substrate.CreateContainer(containerName, _activeCryptoManager, revisionRetentionPolicyType);
        }

        public IContainer GetContainer(string newContainerName)
        {
            VerifyActiveIdentity();
            return GetContainers().Where(s => s.Name == newContainerName).FirstOrDefault();
        }

        public void UnlockContainer(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentException("Must supply a container to unlock");
            }
            if (container.CryptoManagerType != _activeCryptoManager.ManagerType)
            {
                throw new MercurioExceptionRequiredCryptoProviderNotAvailable(string.Format("Container {0} requires crypto provider {1} to unlock, but the current identity {3} does not have it available.", container.Name, container.CryptoManagerType, _activeIdentity.Name));
            }
            // Find the substrate(s) where the container is stored
            var hostSubstrates = _storageSubstrates.Where(s => s.HostsContainer(container.Id));
            // Unlock the container
            foreach (var hostSubstrate in hostSubstrates)
                container.Unlock(hostSubstrate.RetrievePrivateMetadataBytes(container.Id), _activeCryptoManager);
        }

        public void LockContainer(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentException("Must supply a container to lock");
            }
            container.Lock(_activeCryptoManager);
        }

        private void VerifyActiveIdentity()
        {
            if (_activeIdentity == null || _activeCryptoManager == null || _activeCredential == null)
            {
                throw new MercurioException("Active identity must be set");
            }
        }

        public List<UserIdentity> GetAvailableIdentities()
        {
            var identities = new List<UserIdentity>();
            foreach (var cryptographicStorageProvider in _cryptographicServiceProviders)
            {
                var manager = cryptographicStorageProvider.CreateManager(GetCryptoManagerConfiguration(cryptographicStorageProvider));
                identities.AddRange(manager.GetAvailableIdentities());
            }
            return identities;
        }

        private CryptoManagerConfiguration GetCryptoManagerConfiguration(ICryptographicServiceProvider provider)
        {
            var configuration = provider.GetConfiguration();
            if (_userHomeDirectory != null && _userHomeDirectory != "")
            {
                var userEnvironmentConfiguration = new CryptoManagerConfiguration();
                userEnvironmentConfiguration.Add(CryptoConfigurationKeyEnum.KeyringPath.ToString(), _userHomeDirectory);
                configuration.Merge(userEnvironmentConfiguration);            
            }
            return configuration;
        }

        public void SetActiveIdentity(UserIdentity identity)
        {
            if (identity == null)
            {
                _activeCredential = null;
                _activeCryptoManager = null;
                return;
            }

            var cryptoProvider = _cryptographicServiceProviders.Where(s => s.GetProviderType() == identity.CryptoManagerType).FirstOrDefault();
            if (cryptoProvider == null)
            {
                throw new MercurioException(string.Format("Cannot find cryptographic provider for {0} in the current environment", identity.CryptoManagerType));
            }

            var credential = _passphraseFunction(identity.UniqueIdentifier);
            if (credential == null)
            {
                throw new MercurioException(string.Format("Cannot change to requested identity {0} - bad login", identity));
            }
            _activeCredential = credential;

            _activeCryptoManager = cryptoProvider.CreateManager(GetCryptoManagerConfiguration(cryptoProvider));
            _activeCryptoManager.SetCredential(_activeCredential);

            if (_activeCryptoManager.GetFingerprint(identity.UniqueIdentifier) == null)
            {
                throw new MercurioException(string.Format("Specified identity {0} is not available", identity));
            }
            _activeIdentity = identity;
        }
    }
}
