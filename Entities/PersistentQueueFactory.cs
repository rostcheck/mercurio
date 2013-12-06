using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public static class PersistentQueueFactory
    {
        public static IPersistentQueue Create(PeristentQueueType queueType)
        {
            switch (queueType)
            {
                case PeristentQueueType.LocalFileStorage:
                    return new PersistentQueueWithLocalFileStorage();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
