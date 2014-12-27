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
        private List<IStoragePlan> _storagePlans;

        public static MercurioEnvironment Create(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, 
            IEnumerable<IStorageSubstrate> storageSubstrates, 
            IEnumerable<IStoragePlan> storagePlans)
        {
            if (cryptographicServiceProviders == null || storageSubstrates == null || storagePlans == null)
                throw new ArgumentNullException();

            if (!cryptographicServiceProviders.Any())
                throw new ArgumentException("Must provide at least one cryptographic service provider");

            if (!storageSubstrates.Any())
                throw new ArgumentException("Must provide at least one storage substrate");
            
            if (!storagePlans.Any())
                throw new ArgumentException("Must provide at least one storage plan");

            return new MercurioEnvironment(cryptographicServiceProviders, storageSubstrates, storagePlans);
        }

        private MercurioEnvironment(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, 
            IEnumerable<IStorageSubstrate> storageSubstrates,
            IEnumerable<IStoragePlan> storagePlans)
        {
            this._cryptographicServiceProviders = new List<ICryptographicServiceProvider>(cryptographicServiceProviders);
            this._storageSubstrates = new List<IStorageSubstrate>(storageSubstrates);
            this._storagePlans = new List<IStoragePlan>(storagePlans);
        }

        public List<Container> GetContainers()
        {
            var returnList = new List<Container>();
            foreach(var substrate in this._storageSubstrates)
            {
                returnList.AddRange(substrate.GetContainers());
            }
            return returnList;
        }

        public List<string> GetAvailableStorageSubstrateNames()
        {
            return new List<string>(_storageSubstrates.Select(s => s.Name));
        }
       
        public Container CreateContainer(string containerName, string storageSubstrateName, string storagePlanName,
            RevisionRetentionPolicyType revisionRetentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            var substrate = _storageSubstrates.SingleOrDefault(s => s.Name.ToLower() == storageSubstrateName.ToLower());
            if (substrate == null)
            {
                throw new ArgumentException(string.Format("Invalid storage substrate name {0}", storageSubstrateName));
            }

            var storagePlan = _storagePlans.SingleOrDefault(s => s.Name.ToLower() == storagePlanName.ToLower());
            if (storagePlan == null)
            {
                throw new ArgumentException(string.Format("Invalid storage plan name {0}", storagePlanName));
            }

            var container = Container.Create(containerName, storagePlan, revisionRetentionPolicyType);
            substrate.AddContainer(container);
            return container;
        }

        public List<string> GetAvailableStoragePlanNames()
        {
            return new List<string>(_storagePlans.Select(s => s.Name));
        }
    }
}
