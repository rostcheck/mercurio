using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Abstract class (root) for concrete basic data element types
    /// </summary>
    public abstract class AtomicDataElement
    {
        public string Name { get; set; }
        public virtual object Value { get; set; }
        public DataElementType ElementType { get; protected set;  }

        public bool SameElementAs(AtomicDataElement otherElement)
        {
            return (this.Name == otherElement.Name && this.ElementType == otherElement.ElementType);
        }
    }
}
