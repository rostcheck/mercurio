using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FileBasedPersistentQueue : IPersistentQueue
    {
        public void Add(IMercurioMessage message)
        {
            throw new NotImplementedException();
        }

        public IMercurioMessage GetNext()
        {
            throw new NotImplementedException();
        }
    }
}
