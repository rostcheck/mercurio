using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RevisionConflictPolicy
{
    public interface IRevisionConflictPolicy
    {
        Revision GetAuthoritativeRevision(Record record, List<Revision> conflictingRevisions);
    }
}
