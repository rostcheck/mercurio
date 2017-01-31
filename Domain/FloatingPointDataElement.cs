namespace Mercurio.Domain
{
    /// <summary>
    /// Floating point data element.
    /// </summary>
    public class FloatingPointDataElement : IAtomicDataElement
    {
        private double _doubleValue;
        private string _name;

        public FloatingPointDataElement(string name, double value)
        {
            _name = name;
            _doubleValue = value;
        }

        public FloatingPointDataElement(Field field)
        {            
            _name = field.Name;
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
                return DataElementType.FloatingPoint;
            }
        }
    }
}