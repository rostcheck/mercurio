using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Specifies whether old revisions are retained, and if so how many.
    /// </summary>
    public interface IRevisionRetentionPolicy
    {
        List<DocumentVersionMetadata> RevisionsToDelete(List<DocumentVersionMetadata> currentRevisions);
    }
}
