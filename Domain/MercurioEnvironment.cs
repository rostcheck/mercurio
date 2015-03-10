﻿using System;
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

        public static MercurioEnvironment Create(IEnvironmentScanner scanner, Func<string, NetworkCredential> passphraseFunction)
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
            return new MercurioEnvironment(cryptographicServiceProviders, storageSubstrates, passphraseFunction);
        }

        private MercurioEnvironment(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, 
            IEnumerable<IStorageSubstrate> storageSubstrates, Func<string, NetworkCredential> passphraseFunction)
        {
            this._cryptographicServiceProviders = new List<ICryptographicServiceProvider>(cryptographicServiceProviders);
            this._storageSubstrates = new List<IStorageSubstrate>(storageSubstrates);
            this._passphraseFunction = passphraseFunction;
        }

        public List<IContainer> GetContainers()
        {
            VerifyActiveIdentity();

            var returnList = new List<IContainer>();
            foreach(var substrate in this._storageSubstrates)
            {
                returnList.AddRange(substrate.GetAccessibleContainers(_activeIdentity.UniqueIdentifier, _activeCryptoManager));
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
            return substrate.CreateContainer(containerName, _activeIdentity.UniqueIdentifier, _activeCryptoManager, revisionRetentionPolicyType);
        }

        public IContainer GetContainer(string newContainerName)
        {
            VerifyActiveIdentity();
            return GetContainers().Where(s => s.Name == newContainerName).FirstOrDefault();
        }

        public void UnlockContainer(IContainer container)
        {
            if (container.CryptoManagerType != _activeCryptoManager.ManagerType)
            {
                throw new MercurioExceptionRequiredCryptoProviderNotAvailable(string.Format("Container {0} requires crypto provider {1} to unlock, but the current identity {3} does not have it available.", container.Name, container.CryptoManagerType, _activeIdentity.Name));
            }

            container.Unlock(_activeCryptoManager);
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
                var manager = cryptographicStorageProvider.CreateManager(cryptographicStorageProvider.GetConfiguration());
                identities.AddRange(manager.GetAvailableIdentities());
            }
            return identities;
        }

        public void SetActiveIdentity(UserIdentity identity)
        {
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

            _activeCryptoManager = cryptoProvider.CreateManager(cryptoProvider.GetConfiguration());
            _activeCryptoManager.SetCredential(_activeCredential);

            if (_activeCryptoManager.GetFingerprint(identity.UniqueIdentifier) == null)
            {
                throw new MercurioException(string.Format("Specified identity {0} is not available", identity));
            }
            _activeIdentity = identity;
        }
    }
}
