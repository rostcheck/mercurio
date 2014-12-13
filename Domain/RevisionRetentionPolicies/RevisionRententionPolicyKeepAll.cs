using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public class RevisionRententionPolicyKeepAll : IRevisionRetentionPolicy
    {
        public List<Revision> RevisionsToDelete(List<Revision> currentRevisions)
        {
            return new List<Revision>();
        }
    }
}
