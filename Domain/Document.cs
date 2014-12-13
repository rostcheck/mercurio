using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// A collection of Records. Analagous to a file in operating systems.
    /// </summary>
    public class Document
    {
        private List<Record> _recordList;
        private List<Revision> _revisions;
        private List<RecordChange> _uncommittedRevisions;


        public void ChangeRecord(RecordChange recordChange)
        {              
        }
    }
}
