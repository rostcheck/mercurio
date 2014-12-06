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
        public string IdentityUniqueId { get; private set;  }

        public Revision(Guid priorRevisionGuid, string identityUniqueId, List<AtomicDataElementChange> changes)
        {
            this.Id = Guid.NewGuid();
            this.PriorRevisionGuid = priorRevisionGuid;
            this._changeList = new List<AtomicDataElementChange>(changes);
            this.UtcDateTime = DateTimeOffset.UtcNow;
            this.IdentityUniqueId = identityUniqueId;
        }

        public List<AtomicDataElementChange> GetChanges()
        {
            return new List<AtomicDataElementChange>(_changeList);
        }
    }
}
