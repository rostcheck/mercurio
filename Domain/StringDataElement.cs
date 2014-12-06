using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Data element that stores a string
    /// </summary>
    public class StringDataElement : AtomicDataElement
    {
        private string _stringValue;
        public StringDataElement(string name, string inValue)
        {
            this.Name = name;
            _stringValue = inValue;
            this.ElementType = DataElementType.String;
        }

        public new string Value
        {
            get
            {
                return _stringValue;
            }
            set
            {
                _stringValue = value;
            }
        }
    }
}
