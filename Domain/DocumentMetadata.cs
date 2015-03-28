using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Information about a Document
    /// </summary>
    public class DocumentMetadata
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private DocumentMetadata(string documentName)
        {
            Id = Guid.NewGuid();
            Name = documentName;
        }

        public static DocumentMetadata Create(string documentName)
        {
            return new DocumentMetadata(documentName);
        }
    }
}
