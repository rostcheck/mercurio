using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RevisionRetentionPolicies
{
    public class RevisionRetentionPolicyKeepOne : IRevisionRetentionPolicy
    {
        public List<Revision> RevisionsToDelete(List<Revision> currentRevisions)
        {
            var revisionsToDelete = new List<Revision>(currentRevisions);
            revisionsToDelete.Remove(revisionsToDelete.Last());
            return revisionsToDelete;
        }
    }
}
