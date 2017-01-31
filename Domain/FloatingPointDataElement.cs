namespace Mercurio.Domain
{
    /// <summary>
    /// Floating point data element.
    /// </summary>
    public class FloatingPointDataElement : IAtomicDataElement
    {
        private double _doubleValue;
        private Field _field;

        public FloatingPointDataElement(string name, double value)
        {
            _field.Name = name;
            _doubleValue = value;
        }

        public FloatingPointDataElement(Field field)
        {
            _field = field;
        }

        public double Value
        {
            get
            {
                return _doubleValue;
            }
            set
            {
                _doubleValue = value;
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