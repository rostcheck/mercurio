using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DeletePolicy
{
    public class DeletePolicyAllowSpecificEntities : IDeletePolicy
    {
        private List<Identity> _identities;

        public DeletePolicyAllowSpecificEntities(IEnumerable<Identity> identities)
        {
            _identities = identities.ToList();
        }

        public bool CanDelete(string uniqueIdentifier)
        {
            var matchedIdentity = _identities.Where(s => s.UniqueIdentifier == uniqueIdentifier).SingleOrDefault();
            return (matchedIdentity != null);
        }
    }
}
