using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// an Identity that can sign and encrypt (contains both the PrivatePart and PrivatePart of the PublicPrivateKey)
    /// </summary>
    public class UserIdentity : Identity
    {
        protected UserIdentity(string uniqueIdentifier, string name, string address, string description, string cryptoManagerType)
            : base(uniqueIdentifier, name, address, description, cryptoManagerType)
        {
        }

        public UserIdentity(UserIdentity otherIdentity) : base(otherIdentity)
        {
        }

        public new static UserIdentity Create(string uniqueIdentifier, string name, string address, string description, string cryptoManagerType)
        {
            ValidateRequiredString(uniqueIdentifier, "Unique identifier");
            //ValidateRequiredString(publicKey, "Public key");
            ValidateRequiredString(name, "Name");
            return new UserIdentity(uniqueIdentifier, name, address, description, cryptoManagerType);
        }
    }
}
