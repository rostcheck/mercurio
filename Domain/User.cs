using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A person who uses Mercurio. A User may have multiple Identities
    /// </summary>
    public class MercurioUser
    {
        private List<Identity> _identities;
        private string _name;

        private MercurioUser(string name)
        {
            _name = name;
            _identities = new List<Identity>();
        }

        private MercurioUser(string name, List<Identity> identities) 
            : this(name)
        {
            _identities = new List<Identity>(identities);
        }

        public static MercurioUser Create(string name, Identity identity)
        {
            ValidateName(name);
            ValidateIdentity(identity);
            var identityList = new List<Identity>();
            identityList.Add(identity);
            return Create(name, identityList);
        }

        public static MercurioUser Create(string name, List<Identity> identities)
        {
            ValidateName(name);
            ValidateIdentities(identities);
            return new MercurioUser(name, identities);
        }

        private static void ValidateName(string name)
        {
            if (name == null || name == "")
            {
                throw new Exception("Name is required");
            }
        }

        private static void ValidateIdentity(Identity identity)
        {
            if (identity == null)
            {
                throw new Exception("Identity cannot be null");
            }
        }

        private static void ValidateIdentities(List<Identity> identities)
        {
            if (identities == null || identities.Count == 0)
            {
                throw new Exception("Identity list cannot be null or empty");
            }
        }

        public string Name 
        {
            get
            {
                return _name;
            }
        }

        public List<Identity> GetIdentities()
        {
            return new List<Identity>(_identities);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
