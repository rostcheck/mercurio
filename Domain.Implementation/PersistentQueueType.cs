using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public static class PersistentQueueType
    {
		public const string LocalFileStorage = "LocalDisk";
		public const string CloudQueueStorage = "Azure";
    }
}
