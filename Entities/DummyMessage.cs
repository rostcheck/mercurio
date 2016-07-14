using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Mercurio.Domain;

namespace Entities
{
    [Serializable()]
    public class DummyMessage : MercurioMessageBase, IMercurioMessage
    {
		// Just provides base functionality
	}
}
