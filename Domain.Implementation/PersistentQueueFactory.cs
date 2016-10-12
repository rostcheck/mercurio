using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public static class PersistentQueueFactory
    {
        public static IPersistentQueue Create(PeristentQueueType queueType, IPersistentQueueConfiguration configuration, Serializer serializer)
        {
            switch (queueType)
            {
                case PeristentQueueType.LocalFileStorage:
                    return new PersistentQueueWithLocalFileStorage(configuration, serializer);
                case PeristentQueueType.CloudQueueStorage:
                    return new PersistentQueueWithCloudStorage(configuration, serializer);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
