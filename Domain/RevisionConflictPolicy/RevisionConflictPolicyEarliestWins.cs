using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RevisionConflictPolicy
{
    public class RevisionConflictPolicyEarliestWins : IRevisionConflictPolicy
    {
        public Revision GetAuthoritativeRevision(Record record, List<Revision> conflictingRevisions)
        {
            // Throws exception if more than one has the exact same time
            var earliestUtcDateTime = conflictingRevisions.Min(s => s.UtcDateTime);
            return conflictingRevisions.Where(s => s.UtcDateTime == earliestUtcDateTime).SingleOrDefault();
        }
    }
}
