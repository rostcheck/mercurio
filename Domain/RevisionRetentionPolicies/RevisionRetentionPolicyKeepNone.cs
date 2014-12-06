using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RevisionRetentionPolicies
{
    public class RevisionRetentionPolicyKeepNone : IRevisionRetentionPolicy
    {
        public List<Revision> RevisionsToDelete(List<Revision> currentRevisions)
        {
            return currentRevisions;
        }
    }
}
