using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents an operating environment in which you can do persistent Mercurio storage operations
    /// </summary>
    public class MercurioEnvironment
    {
        private List<ICryptographicServiceProvider> _cryptographicServiceProviders;
        private List<IStorageSubstrate> _storageSubstrates;

        public static MercurioEnvironment Create(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, IEnumerable<IStorageSubstrate> storageSubstrates)
        {
            if (cryptographicServiceProviders == null || storageSubstrates == null)
                throw new ArgumentNullException();

            return new MercurioEnvironment(cryptographicServiceProviders, storageSubstrates);
        }

        private MercurioEnvironment(IEnumerable<ICryptographicServiceProvider> cryptographicServiceProviders, IEnumerable<IStorageSubstrate> storageSubstrates)
        {
            this._cryptographicServiceProviders = new List<ICryptographicServiceProvider>(cryptographicServiceProviders);
            this._storageSubstrates = new List<IStorageSubstrate>(storageSubstrates);
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

        public Container CreateContainer(string containerName, string storageSubstrateName, 
            RevisionRetentionPolicyType revisionRetentionPolicyType = RevisionRetentionPolicyType.KeepOne)
        {
            var substrate = _storageSubstrates.SingleOrDefault(s => s.Name.ToLower() == storageSubstrateName.ToLower());
            if (substrate == null)
            {
                throw new ArgumentException(string.Format("Invalid storage substrate name {0}", storageSubstrateName));
            }

            var container = Container.Create(containerName, revisionRetentionPolicyType);
            substrate.AddContainer(container);
            return container;
        }
    }
}
