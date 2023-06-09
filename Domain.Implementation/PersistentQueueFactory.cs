﻿using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
	public class PersistentQueueFactory : IPersistentQueueFactory
    {
        public IPersistentQueue Create(PersistentQueueConfiguration configuration, Serializer serializer)
        {
			switch (configuration.ServiceType)
            {
                case PersistentQueueType.LocalFileStorage:
                    return new PersistentQueueWithLocalFileStorage(configuration, serializer);
                case PersistentQueueType.CloudQueueStorage:
                    return new PersistentQueueWithCloudStorage(configuration, serializer);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
