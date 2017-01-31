using System;
namespace Mercurio.Domain
{
    /// <summary>
    /// Date time data element.
    /// </summary>
    public class DateTimeDataElement : IAtomicDataElement
    {
        private DateTime _dateTimeValue;
        private Field _field;

        public DateTimeDataElement(string name, DateTime value)
        {
            _field.Name = name;
            _dateTimeValue = value;
        }

        public DateTimeDataElement(Field field)
        {
            _field = field;
        }

        public DateTime Value
        {
            get
            {
                return _dateTimeValue;
            }
            set
            {
                _dateTimeValue = value;
            }
        }

        public string Name
        {
            get
            {
                return _field.Name;
            }
            set
            {
                _field.Name = value;
            }
        }

        public DataElementType ElementType
        {
            get
            {
                return _field.ElementType;
            }
        }
    }
}
