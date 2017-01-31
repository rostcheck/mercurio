namespace Mercurio.Domain
{
    /// <summary>
    /// Integer data element.
    /// </summary>
    public class IntegerDataElement : IAtomicDataElement
    {
        private int _intValue;
        private string _name;

        public IntegerDataElement(string name, int value)
        {
            _name = name;
            _intValue = value;
        }

        public IntegerDataElement(Field field)
        {
            _name = field.Name;
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
                return DataElementType.Integer;
            }
        }
    }
}
