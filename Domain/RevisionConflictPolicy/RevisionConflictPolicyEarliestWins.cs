using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.RevisionConflictPolicy
{
    public class RevisionConflictPolicyEarliestWins : IRevisionConflictPolicy
    {
        public DocumentVersion ResolveConflicingRevisions(Record record, List<DocumentVersion> conflictingRevisions)
        {
            // Throws exception if more than one has the same time
            var earliestUtcDateTime = conflictingRevisions.Min(s => s.CreatedDateTime);
            return conflictingRevisions.Where(s => s.CreatedDateTime == earliestUtcDateTime).SingleOrDefault();
        }
    }
}
