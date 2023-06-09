﻿using System;
namespace Mercurio.Domain
{
    /// <summary>
    /// Date time data element.
    /// </summary>
    public class DateTimeDataElement : IAtomicDataElement
    {
        private DateTime _dateTimeValue;
        private string _name;

        public DateTimeDataElement(string name, DateTime value)
        {
            _name = name;
            _dateTimeValue = value;
        }

        public DateTimeDataElement(Field field)
        {
            _name = field.Name;
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
                return DataElementType.DateTime;
            }
        }
    }
}
