using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public static class PersistentQueueFactory
    {
        public static IPersistentQueue Create(PeristentQueueType queueType, Serializer serializer)
        {
            switch (queueType)
            {
                case PeristentQueueType.LocalFileStorage:
                    return new PersistentQueueWithLocalFileStorage(serializer);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
