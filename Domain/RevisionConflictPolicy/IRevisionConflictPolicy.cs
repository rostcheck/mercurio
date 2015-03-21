using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.RevisionConflictPolicy
{
    public interface IRevisionConflictPolicy
    {
        DocumentVersion ResolveConflicingRevisions(Record record, List<DocumentVersion> conflictingRevisions);
    }
}
