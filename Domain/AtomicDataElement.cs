using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Abstract class (root) for concrete basic data element types
    /// </summary>
    public abstract class AtomicDataElement
    {
        public string Name { get; set; }
        public virtual object Value { get; set; }
        public DataElementType ElementType { get; protected set;  }
    }
}
