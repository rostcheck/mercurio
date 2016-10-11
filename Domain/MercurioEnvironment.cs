using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents an operating environment in which you can do persistent Mercurio storage operations
    /// </summary>
    public class MercurioEnvironment : IMercurioEnvironment
    {
        private List<ICryptographicServiceProvider> _cryptographicServiceProviders;
        private List<IStorageSubstrate> _storageSubstrates;
        private UserIdentity _activeIdentity;
        private ICryptoManager _activeCryptoManager;
        private Func<string, NetworkCredential> _passphraseFunction;
        private NetworkCredential _activeCredential;
        private string _userHomeDirectory = null;
        private ITempStorageSubstrate _tempStorageSubstrate;
        private IOSAbstractor _osAbstractor;
		private string _editor;

        public static MercurioEnvironment Create(IEnvironmentScanner scanner, IOSAbstractor osAbstractor, Serializer serializer, Func<string, NetworkCredential> passphraseFunction)
        {
            var cryptographicServiceProviders = scanner.GetCryptographicProviders();
            var storageSubstrates = scanner.GetStorageSubstrates();
            var tempStorageSubstrate = scanner.GetTemporaryStorageSubstrate();
            var editor = scanner.GetEditor();

            if (cryptographicServiceProviders == null || storageSubstrates == null)
                throw new ArgumentNullException();

            if (!cryptographicServiceProviders.Any())
                throw new ArgumentException("Must provide at least one cryptographic service provider");

            if (!storageSubstrates.Any())
                throw new ArgumentException("Must provide at least one storage substrate");

            if (tempStorageSubstrate == null)
                throw new ArgumentException("Cannot construct a temporary storage substrate");

            if (editor == null)
                throw new ArgumentException("Cannot locate an editor");

            if (passphraseFunction == null)
            {
                throw new ArgumentNullException("Must provide a valid passphrase function");
            }
            return new MercurioEnvironment(cryptographicServiceProviders, osAbstractor, serializer, storageSubstrates, tempStorageSubstrate, editor, passphraseFunction);
        }

        private MercurioEnvironment(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, 
            IOSAbstractor osAbstractor,
            Serializer serializer,
            IEnumerable<IStorageSubstrate> storageSubstrates, 
            ITempStorageSubstrate tempStorageSubstrate, 
            string editor,
            Func<string, NetworkCredential> passphraseFunction)
        {
            this._cryptographicServiceProviders = new List<ICryptographicServiceProvider>(cryptographicServiceProviders);
            this._storageSubstrates = storageSubstrates.ToList();
            this._passphraseFunction = passphraseFunction;
            //this._serializer = serializer;
            this._tempStorageSubstrate = tempStorageSubstrate;
            this._editor = editor;
            this._osAbstractor = osAbstractor;
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
                returnList.AddRange(substrate.GetAllContainers());
            }
            return returnList;
        }

        public IStorageSubstrate GetSubstrateHostingContainer(string containerName)
        {
            VerifyActiveIdentity();

            foreach (var substrate in this._storageSubstrates)
            {
                var container = substrate.GetAllContainers().FirstOrDefault(s => s.Name.ToLower() == containerName.ToLower());
                if (container != null)
                    return substrate;
            }
            return null;
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
                throw new ArgumentException(string.Format("Invalid storage substrate name {0}", storageSubstrateName));

            if (GetContainers().Where(s => s.Name.ToLower() == containerName.ToLower()).FirstOrDefault() != null)
                throw new ArgumentException(string.Format("Container {0} already exists in the environment", containerName));

            return substrate.CreateContainer(containerName, _activeCryptoManager, revisionRetentionPolicyType);
        }

        public void DeleteContainer(string containerName)
        {
            var container = GetContainer(containerName);
            if (container == null)
                throw new ArgumentException(string.Format("Cannot find a container with name {0}", containerName));

            var substrate = GetSubstrateHostingContainer(containerName);
            if (substrate == null)
                throw new ArgumentException(string.Format("Cannot identify substrate hosting container with name {0}", containerName));

            substrate.DeleteContainer(container.Id);
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
                var manager = cryptographicStorageProvider.CreateManager(GetCryptoManagerConfiguration(cryptographicStorageProvider, _osAbstractor));
                identities.AddRange(manager.GetAvailableIdentities());
            }
            return identities;
        }

        public UserIdentity GetUserIdentity(string identifier)
        {
            var identities = new List<UserIdentity>();
            foreach (var cryptographicStorageProvider in _cryptographicServiceProviders)
            {
                var manager = cryptographicStorageProvider.CreateManager(GetCryptoManagerConfiguration(cryptographicStorageProvider, _osAbstractor));
                identities.AddRange(manager.GetAvailableIdentities());
            }
            return identities.Where(s => s.UniqueIdentifier == identifier).FirstOrDefault();
        }

        private CryptoManagerConfiguration GetCryptoManagerConfiguration(ICryptographicServiceProvider provider, IOSAbstractor osAbstractor)
        {
            var configuration = provider.GetConfiguration(osAbstractor);
            if (_userHomeDirectory != null && _userHomeDirectory != "")
            {
                var userEnvironmentConfiguration = new CryptoManagerConfiguration();
                userEnvironmentConfiguration.Add(CryptoConfigurationKeyEnum.KeyringPath.ToString(), _userHomeDirectory);
                configuration.Merge(userEnvironmentConfiguration);            
            }
            return configuration;
        }

        public UserIdentity GetActiveIdentity()
        {
            return new UserIdentity(_activeIdentity);
        }

        public bool ConfirmActiveIdentity()
        {
            if (_activeIdentity == null)
                throw new MercurioExceptionIdentityNotSet("No active identity is set");

            var credential = _passphraseFunction(_activeIdentity.UniqueIdentifier);
            if (credential == null)
                return false;

            var cryptoProvider = _cryptographicServiceProviders.Where(s => s.GetProviderType() == _activeIdentity.CryptoManagerType).FirstOrDefault();
            if (cryptoProvider == null)
            {
                throw new MercurioExceptionRequiredCryptoProviderNotAvailable(string.Format("Cannot find cryptographic provider for {0} in the current environment", _activeIdentity.CryptoManagerType));
            }

            var cryptoManager = cryptoProvider.CreateManager(GetCryptoManagerConfiguration(cryptoProvider, _osAbstractor));
            if (cryptoManager.GetFingerprint(_activeIdentity.UniqueIdentifier) == null)
            {
                throw new MercurioExceptionNoIdentitiesAvailable(string.Format("Specified identity {0} is not available", _activeIdentity.Name));
            }

            return cryptoManager.ValidateCredential(credential);
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
                throw new MercurioExceptionRequiredCryptoProviderNotAvailable(string.Format("Cannot find cryptographic provider for {0} in the current environment", identity.CryptoManagerType));
            }

            var credential = _passphraseFunction(identity.UniqueIdentifier);
            if (credential == null)
            {
                throw new MercurioExceptionIdentityNotSet(string.Format("Cannot change to requested identity {0} - bad login", identity.Name));
            }

            var cryptoManager = cryptoProvider.CreateManager(GetCryptoManagerConfiguration(cryptoProvider, _osAbstractor));
            if (cryptoManager.GetFingerprint(identity.UniqueIdentifier) == null)
            {
                throw new MercurioExceptionNoIdentitiesAvailable(string.Format("Specified identity {0} is not available", identity.Name));
            }
            if (!cryptoManager.ValidateCredential(credential))
            {
                throw new MercurioExceptionIdentityNotSet(string.Format("Cannot change to requested identity {0} - invalid password", identity.Name));
            }
            cryptoManager.SetCredential(credential);


            _activeIdentity = identity;
            _activeCredential = credential;
            _activeCryptoManager = cryptoManager;
        }

        public string EditDocument(string fileName, string clearTextContent)
        {           
            _tempStorageSubstrate.StoreData(fileName, clearTextContent);
            var process = new Process();
            process.StartInfo.Arguments = _tempStorageSubstrate.GetPath(fileName);
			process.StartInfo.FileName = _editor;
            process.Start();
            process.WaitForExit();
            var result = _tempStorageSubstrate.RetrieveData(fileName);
            _tempStorageSubstrate.EraseData(fileName);
            return result;
        }

		public IPersistentQueue CreateQueue(string name, string serviceType)
		{
			throw new NotImplementedException();
		}
    }
}
