using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// an Identity that also contains an address to which Mercurio communications can be targeted
    /// </summary>
    public class ContactIdentity : Identity
    {
        protected ContactIdentity(string uniqueIdentifier, string name, string address, string description, string cryptoManagerType)
            : base (uniqueIdentifier, name, address, description, cryptoManagerType)
        {
        }

        public static new ContactIdentity Create(string uniqueIdentifier, string name, string address, string description, string cryptoManagerType)
        {
            ValidateRequiredString(uniqueIdentifier, "Unique identifier");
            //ValidateRequiredString(publicKey, "Public key");
            ValidateRequiredString(name, "Name");
            if (address == null || address == "")
            {
                address = string.Format("{0}@local", uniqueIdentifier);
            }
            return new ContactIdentity(uniqueIdentifier, name, address, description, cryptoManagerType);
        }
    }
}
