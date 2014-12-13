using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.DeletePolicy
{
    public class DeletePolicyAllowAll : IDeletePolicy
    {
        public bool CanDelete(string uniqueIdentifier)
        {
            return true;
        }
    }
}
