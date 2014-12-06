using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ChangePolicy
{
    /// <summary>
    /// Specifies which Identities can change Records in the Container
    /// </summary>
    public interface IChangePolicy
    {
        bool CanMakeChange(string uniqueIdentifier);
    }
}
