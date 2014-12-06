using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ChangePolicy
{
    public class ChangePolicyAllowAll : IChangePolicy
    {
        public bool CanMakeChange(string uniqueIdentifier)
        {
            return true;
        }
    }
}
