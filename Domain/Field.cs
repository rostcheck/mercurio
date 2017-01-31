using System;
namespace Mercurio.Domain
{
    /// <summary>
    /// An individual field (structure: name, data type, but not value) of a database record
    /// </summary>
    public class Field
    {
        public string Name { get; set; }
        public DataElementType ElementType { get; protected set; }

        public Field(string name, DataElementType elementType)
        {
            Name = name;
            ElementType = elementType;
        }
    }
}
