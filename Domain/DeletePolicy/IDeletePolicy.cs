using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DeletePolicy
{
    /// <summary>
    /// Specifies which Identities can change Records in the Container
    /// </summary>
    public interface IDeletePolicy
    {
        bool CanDelete(string identityUniqueId);
    }
}
