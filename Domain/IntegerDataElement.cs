namespace Mercurio.Domain
{
    /// <summary>
    /// Integer data element.
    /// </summary>
    public class IntegerDataElement : IAtomicDataElement
    {
        private int _intValue;
        private Field _field;

        public IntegerDataElement(string name, int value)
        {
            _field.Name = name;
            _intValue = value;
        }

        public IntegerDataElement(Field field)
        {
            _field = field;
        }

        public int Value
        {
            get
            {
                return _intValue;
            }
            set
            {
                _intValue = value;
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
