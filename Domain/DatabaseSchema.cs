using System.Collections.Generic;

namespace Mercurio.Domain
{
    public class DatabaseSchema
    {
        private List<Field> _fields;
        public List<Field> Fields
        {
            get
            {
                return new List<Field>(_fields);
            }
        }
        public DatabaseSchema()
        {
        }

        public void AddField(Field field)
        {
            if (_fields == null)
                _fields = new List<Field>();
            
            _fields.Add(field);
        }
   
    }
}
