using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DeletePolicy
{
    public class DeletePolicyAllowNone : IDeletePolicy
    {
        public bool CanDelete(string uniqueIdentifier)
        {
            return false;
        }
    }
}
