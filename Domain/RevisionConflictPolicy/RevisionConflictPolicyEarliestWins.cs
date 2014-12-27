﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.RevisionConflictPolicy
{
    public class RevisionConflictPolicyEarliestWins : IRevisionConflictPolicy
    {
        public Revision ResolveConflicingRevisions(Record record, List<Revision> conflictingRevisions)
        {
            // Throws exception if more than one has the same time
            var earliestUtcDateTime = conflictingRevisions.Min(s => s.UtcDateTime);
            return conflictingRevisions.Where(s => s.UtcDateTime == earliestUtcDateTime).SingleOrDefault();
        }
    }
}