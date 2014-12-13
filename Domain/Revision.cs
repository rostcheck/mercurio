using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A change to a Record. Revisions are immutable; changing a Record produces a new Revision.
    /// </summary>
    public class Revision
    {
        public Guid Id { get; private set;  }
        public Guid PriorRevisionGuid { get; private set;  }
        private List<AtomicDataElementChange> _changeList;
        public string DocumentContent { get; private set; }
        public DateTimeOffset UtcDateTime { get; private set;  }
        public string RevisorIdentityUniqueId { get; private set;  }

        private Revision(Guid priorRevisionGuid, string revisorIdentityUniqueId)
        {
            this.Id = Guid.NewGuid();
            this.PriorRevisionGuid = priorRevisionGuid;
            this.UtcDateTime = DateTimeOffset.UtcNow;
            this.RevisorIdentityUniqueId = revisorIdentityUniqueId;
        }

        private Revision(Guid priorRevisionGuid, string revisorIdentityUniqueId, List<AtomicDataElementChange> changes)
            : this(priorRevisionGuid, revisorIdentityUniqueId)
        {
            this._changeList = new List<AtomicDataElementChange>(changes);
        }

        private Revision(Guid priorRevisionGuid, string revisorIdentityUniqueId, string documentContent)
            : this(priorRevisionGuid, revisorIdentityUniqueId)
        {
            this.DocumentContent = documentContent;
        }

        public List<AtomicDataElementChange> GetChanges()
        {
            return new List<AtomicDataElementChange>(_changeList);
        }

        internal static Revision Create(Guid priorRevisionGuid, string revisorIdentityUniqueId, List<AtomicDataElementChange> changes)
        {
            return new Revision(priorRevisionGuid, revisorIdentityUniqueId, changes);
        }

        internal static Revision Create(Guid priorRevisionGuid, string revisiorIdentityUniqueId, string documentContent)
        {
            return new Revision(priorRevisionGuid, revisiorIdentityUniqueId, documentContent);
        }
    }
}
