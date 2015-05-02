﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.TestMocks
{
    public class MockMercurioEnvironment : IMercurioEnvironment
    {
        private string _userHomeDirectory;
        private List<IContainer> _containers;
        private List<IStorageSubstrate> _storageSubstrates;
        private UserIdentity _activeUserIdentity;
        private ICryptoManager _cryptoManager;

        public MockMercurioEnvironment()
        {
            _containers = new List<IContainer>();
            _storageSubstrates = new List<IStorageSubstrate>() { new InMemoryStorageSubstrate() };
            _cryptoManager = new MockCryptoManager();
        }

        public void SetUserHomeDirectory(string userHomeDirectory)
        {
            _userHomeDirectory = userHomeDirectory;
        }

        public List<IContainer> GetContainers()
        {
            return new List<IContainer>(_containers);
        }

        public List<string> GetAvailableStorageSubstrateNames()
        {
            return _storageSubstrates.Select(s => s.Name).ToList();
        }

        public IContainer CreateContainer(string containerName, string storageSubstrateName, RevisionRetentionPolicyType revisionRetentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            var substrate =  _storageSubstrates.Where(s => s.Name == storageSubstrateName).FirstOrDefault();
            if (substrate == null)
                throw new MercurioException("Unrecognized storage substrate");

            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var container = Container.Create(containerName, _cryptoManager, substrate, serializer, revisionRetentionPolicyType);
            _containers.Add(container);
            return container;
        }

        public IContainer GetContainer(string newContainerName)
        {
            throw new NotImplementedException();
        }

        public void UnlockContainer(IContainer container)
        {
            throw new NotImplementedException();
        }

        public void LockContainer(IContainer container)
        {
            throw new NotImplementedException();
        }

        public List<UserIdentity> GetAvailableIdentities()
        {
            return _cryptoManager.GetAvailableIdentities();
        }

        public void SetActiveIdentity(UserIdentity identity)
        {
            _cryptoManager.SetCredential(TestUtilities.TestUtils.PassphraseFunction(identity.UniqueIdentifier));
        }
    }
}