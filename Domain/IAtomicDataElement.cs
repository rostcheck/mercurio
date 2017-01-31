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
    public interface IAtomicDataElement
    {
        string Name { get; set; }
        DataElementType ElementType { get; }
        // Should also declare a "Value"
    }
}
