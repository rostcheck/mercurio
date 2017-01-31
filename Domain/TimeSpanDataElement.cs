using System;
namespace Mercurio.Domain
{
    /// <summary>
    /// Time span data element.
    /// </summary>
    public class TimeSpanDataElement : IAtomicDataElement
    {
        private TimeSpan _timeSpanValue;
        private string _name;

        public TimeSpanDataElement(string name, TimeSpan value)
        {
            _name = name;
            _timeSpanValue = value;
        }

        public TimeSpanDataElement(Field field)
        {
            _name = field.Name;
        }

        public TimeSpan Value
        {
            get
            {
                return _timeSpanValue;
            }
            set
            {
                _timeSpanValue = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public DataElementType ElementType
        {
            get
            {
                return DataElementType.TimeSpan;
            }
        }
    }
}
