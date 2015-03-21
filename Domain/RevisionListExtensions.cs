using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public static class RevisionListExtensions
    {
        public static List<DocumentVersionMetadata> WithoutExcessRevisions(this List<DocumentVersionMetadata> revisions, RevisionRetentionPolicyType retentionPolicyType)
        {
            var retentionPolicy = RevisionRetentionPolicy.Create((RevisionRetentionPolicyType)retentionPolicyType);
            var revisionsToExclude = retentionPolicy.RevisionsToDelete(revisions).Select(s => s.Id).ToArray();
            return revisions.Where(s => !revisionsToExclude.Contains(s.Id)).ToList();
        }
    }
}
