using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.RevisionConflictPolicy
{
    public interface IRevisionConflictPolicy
    {
        Revision ResolveConflicingRevisions(Record record, List<Revision> conflictingRevisions);
    }
}
