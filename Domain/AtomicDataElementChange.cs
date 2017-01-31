using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents a change (set value, delete value) to an AtomicDataElement
    /// </summary>
    public class AtomicDataElementChange
    {        
        public ChangeType ChangeType { get; private set; }
        public string Name;
        public object Value;
        public DataElementType ElementType;

        private AtomicDataElementChange()
        {
        }

        public static AtomicDataElementChange SetValue(string name, object value, DataElementType elementType)
        {
            return new AtomicDataElementChange()
            {
                Name = name,
                Value = value,
                ElementType = elementType,
                ChangeType = ChangeType.Set
            };
        }

        public static AtomicDataElementChange DeleteValue(string name, DataElementType elementType)
        {
            return new AtomicDataElementChange()
            {
                Name = name,
                ElementType = elementType,
                ChangeType = ChangeType.Delete
            };
        }
    }
}
