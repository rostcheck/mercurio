using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    ///  A unique association of atomic data elements.
    /// </summary>
    public class Record
    {
        private List<Revision> _revisions;
        private List<AtomicDataElement> _dataElements;

        public List<Revision> GetRevisions()
        {
            throw new NotImplementedException();
        }

        public Record()
        {
            _revisions = new List<Revision>();
        }
       
        public List<AtomicDataElement> GetDataElements()
        {
            throw new NotImplementedException();
        }

        public void BeginRevision()
        {
            throw new NotImplementedException();
        }

        public void ChangeElement(AtomicDataElement dataElement)
        {
            throw new NotImplementedException();
        }

        public void DeleteElement(string name, DataElementType elementType)
        {
            throw new NotImplementedException();
        }
    }
}
