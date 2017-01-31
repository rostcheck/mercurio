﻿namespace Mercurio.Domain
{
    /// <summary>
    /// String data element.
    /// </summary>
    public class StringDataElement : IAtomicDataElement
    {
        private string _stringValue;
        private Field _field;

        public StringDataElement(string name, string value)
        {
            _field.Name = name;
            _stringValue = value;
        }

        public StringDataElement(Field field)
        {
            _field = field;
        }

        public string Value
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
