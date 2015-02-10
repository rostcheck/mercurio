using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class DiskDirectoryNode
    {
        public string FileName { get; private set; }
        public int Revision { get; private set; }

        private DiskDirectoryNode(string fileName, int revision)
        {
            this.FileName = fileName;
            this.Revision = revision;
        }

        public static DiskDirectoryNode Create (string fileName, int revision)
        {
            return new DiskDirectoryNode(fileName, revision);
        }
    }
}
