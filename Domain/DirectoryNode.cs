using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public class DirectoryNode
    {
        public string FileName { get; private set; }
        public int Revision { get; private set; }

        private DirectoryNode(string fileName, int revision)
        {
            this.FileName = fileName;
            this.Revision = revision;
        }

        public static DirectoryNode Create (string fileName, int revision)
        {
            return new DirectoryNode(fileName, revision);
        }
    }
}
