using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A unique cryptographic key.
    /// </summary>
    public class Identity
    {
        public string UniqueIdentifier { get; private set; }
        public string PublicPart { get; private set; }
        public string PrivatePart { get; private set; }

        private Identity(string uniqueIdentifier, string publicKey, string privateKey = "")
        {
            this.UniqueIdentifier = uniqueIdentifier;
            this.PublicPart = publicKey;
            this.PrivatePart = privateKey;
        }

        public static Identity Create(string uniqueIdentifier, string publicKey, string privateKey = "")
        {
            ValidateUniqueIdentifier(uniqueIdentifier);
            ValidatePublicKey(publicKey);
            return new Identity(uniqueIdentifier, publicKey, privateKey);
        }

        private static void ValidateUniqueIdentifier(string uniqueIdentifier)
        {
            if (uniqueIdentifier == null || uniqueIdentifier == "")
            {
                throw new Exception("UniqueIdentifier is required");
            }
        }

        private static void ValidatePublicKey(string publicKey)
        {
            if (publicKey == null || publicKey == "")
            {
                throw new Exception("Public key is required");
            }
        }

        public override bool Equals(object obj)
        {
            var otherIdentity = obj as Identity;
            return (otherIdentity == null) ? false : (this.UniqueIdentifier == otherIdentity.UniqueIdentifier);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
