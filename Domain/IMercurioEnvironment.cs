using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public interface IMercurioEnvironment
    {
        void SetUserHomeDirectory(string userHomeDirectory);
        List<IContainer> GetContainers();
        List<string> GetAvailableStorageSubstrateNames();
        IContainer CreateContainer(string containerName, string storageSubstrateName,
            RevisionRetentionPolicyType revisionRetentionPolicyType = RevisionRetentionPolicyType.KeepOne);
        IContainer GetContainer(string newContainerName);
        void DeleteContainer(string containerName);
        void UnlockContainer(IContainer container);
        void LockContainer(IContainer container);
        List<UserIdentity> GetAvailableIdentities();
        void SetActiveIdentity(UserIdentity identity);
        UserIdentity GetActiveIdentity();
        bool ConfirmActiveIdentity();
    }
}
