using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Specifies whether old revisions are retained, and if so how many.
    /// </summary>
    public interface IRevisionRetentionPolicy
    {
        List<Revision> RevisionsToDelete(List<Revision> currentRevisions);
    }
}
