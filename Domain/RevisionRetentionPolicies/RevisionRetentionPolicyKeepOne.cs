using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public class RevisionRetentionPolicyKeepOne : IRevisionRetentionPolicy
    {
        public List<DocumentVersionMetadata> RevisionsToDelete(List<DocumentVersionMetadata> currentRevisions)
        {
            var revisionsToDelete = new List<DocumentVersionMetadata>(currentRevisions);
            revisionsToDelete.Remove(revisionsToDelete.Last());
            return revisionsToDelete;
        }
    }
}
