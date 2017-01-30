using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents a change (add, change, or delete) to a record
    /// </summary>
    public class RecordChange
    {
        public ChangeType ChangeType { get; private set; }
//        private List<AtomicDataElementChange> _changes;

        public RecordChange()
        {
        }
    }
}
