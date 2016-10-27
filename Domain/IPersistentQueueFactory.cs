using System;

namespace Mercurio.Domain
{
	public interface IPersistentQueueFactory
	{
		IPersistentQueue Create(PersistentQueueConfiguration configuration, Serializer serializer);
	}
}

