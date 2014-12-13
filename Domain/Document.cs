using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// A collection of Records. Analagous to a file in operating systems.
    /// </summary>
    public class Document
    {
        private Document()
        {
        }
    
        public void ChangeRecord(RecordChange recordChange)
        {              
        }

        internal static TextDocument CreateTextDocument(string documentName, IRevisionRetentionPolicy retentionPolicy, Identity creatorIdentity, string initialData = null)
        {
            return TextDocument.Create(documentName, retentionPolicy, creatorIdentity, initialData);
        }
    }
}
