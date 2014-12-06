using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AtomicDataElementChange : AtomicDataElement
    {
        public virtual object OldValue { get; set; }
    }
}
