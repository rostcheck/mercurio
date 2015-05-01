using System;
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

        public MockMercurioEnvironment()
        {
            _containers = new List<IContainer>();
            _storageSubstrates = new List<IStorageSubstrate>();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetActiveIdentity(UserIdentity identity)
        {
            _activeUserIdentity = identity;
        }
    }
}
