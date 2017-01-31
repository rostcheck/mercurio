namespace Mercurio.Domain
{
    /// <summary>
    /// String data element.
    /// </summary>
    public class StringDataElement : IAtomicDataElement
    {
        private string _stringValue;
        private string _name;

        public StringDataElement(string name, string value)
        {
            _name = name;
            _stringValue = value;
        }

        public StringDataElement(Field field)
        {
            _name = field.Name;
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
                return DataElementType.String;
            }
        }
    }
}
