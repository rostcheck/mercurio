using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public static class RevisionRetentionPolicy
    {
        public static IRevisionRetentionPolicy Create(RevisionRetentionPolicyType retentionPolicyType)
        {
            switch (retentionPolicyType)
            {
                case RevisionRetentionPolicyType.KeepAll:
                    return new RevisionRententionPolicyKeepAll();
                case RevisionRetentionPolicyType.KeepOne:
                    return new RevisionRetentionPolicyKeepOne();
                default:
                    throw new Exception(string.Format("Unknown revision retention policy type {0}", retentionPolicyType));
            }
        }
    }
}
