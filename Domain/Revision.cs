using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// A change to a Record. Revisions are immutable; changing a Record produces a new Revision.
    /// </summary>
    public class Revision
    {
        public Guid Id { get; private set;  }
        public Guid PriorRevisionGuid { get; private set;  }
        private List<AtomicDataElementChange> _changeList;
        public DateTimeOffset UtcDateTime { get; private set;  }
        public string RevisorIdentityUniqueId { get; private set;  }

        private Revision(Guid priorRevisionGuid, string revisorIdentityUniqueId, List<AtomicDataElementChange> changes)
        {
            this.Id = Guid.NewGuid();
            this.PriorRevisionGuid = priorRevisionGuid;
            this._changeList = new List<AtomicDataElementChange>(changes);
            this.UtcDateTime = DateTimeOffset.UtcNow;
            this.RevisorIdentityUniqueId = revisorIdentityUniqueId;
        }

        public List<AtomicDataElementChange> GetChanges()
        {
            return new List<AtomicDataElementChange>(_changeList);
        }

        internal static Revision Create(Guid priorRevisionGuid, string revisorIdentityUniqueId, List<AtomicDataElementChange> changes)
        {
            return new Revision(priorRevisionGuid, revisorIdentityUniqueId, changes);
        }
    }
}
