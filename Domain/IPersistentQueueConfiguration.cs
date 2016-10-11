using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public interface IPersistentQueueConfiguration
    {
		string Name { get; set; }
        string ConfigurationString { get; set; }
    }
}
