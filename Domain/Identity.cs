﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A unique cryptographic key identified by the cryptographic PublicPart of a PublicPrivateKey. 
    /// </summary>
    public class Identity
    {
        public string UniqueIdentifier { get; private set; }
        public string Description { get; private set; } // ex. "John's Personal Account"
        public string Address { get; private set; } // address for message delivery (ex. person@mercurio.org)
        public string Name { get; private set; } // readable name (ex. "John Smith")
        public string CryptoManagerType { get; private set; } // Identity must be established by an ICryptoManager
		public DateTime? ExpirationDate { get; private set; } // Ex: 2014-08-12

        protected Identity(string uniqueIdentifier, string name, string address, string description, string cryptoManagerType, DateTime? expirationDate = null)
        {
            this.UniqueIdentifier = uniqueIdentifier;
            this.Name = name;
            this.Address = address;
            this.Description = description;
            this.CryptoManagerType = cryptoManagerType;
			this.ExpirationDate = expirationDate;
        }

        protected Identity(Identity otherIdentity)
        {
            this.UniqueIdentifier = otherIdentity.UniqueIdentifier;
            this.Name = otherIdentity.Name;
            this.Address = otherIdentity.Address;
            this.Description = otherIdentity.Description;
            this.CryptoManagerType = otherIdentity.CryptoManagerType;
        }

        public static Identity Create(string uniqueIdentifier, string name, string address, string description, string cryptoManagerType, DateTime? expirationDate = null)
        {
            ValidateRequiredString(uniqueIdentifier, "Unique identifier");
            //ValidateRequiredString(publicKey, "Public key");
            ValidateRequiredString(name, "Name");
            if (address == null || address == "")
            {
                address = string.Format("{0}@local", uniqueIdentifier);
            }
            return new Identity(uniqueIdentifier, name, address, description, cryptoManagerType, expirationDate);
        }

        protected static void ValidateRequiredString(string requiredString, string name)
        {
            if (requiredString == null || requiredString == "")
            {
                throw new Exception(string.Format("{0} is required", name));
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
			return (otherIdentity == null) ? false : (this.UniqueIdentifier == otherIdentity.UniqueIdentifier && this.ExpirationDate == otherIdentity.ExpirationDate);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
