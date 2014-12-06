using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ChangePolicy
{
    public class ChangePolicyAllowSpecificIdentities : IChangePolicy
    {
        private List<Identity> _identities;

        public ChangePolicyAllowSpecificIdentities(IEnumerable<Identity> identities)
        {
            _identities = identities.ToList();
        }

        public bool CanMakeChange(string uniqueIdentifier)
        {
            var matchedIdentity = _identities.Where(s => s.UniqueIdentifier == uniqueIdentifier).SingleOrDefault();
            return (matchedIdentity != null);
        }
    }
}
