using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    public interface IPersistentQueue
    {
		string Name { get; }
        void Add(EnvelopedMercurioMessage message);
        EnvelopedMercurioMessage GetNext(string address);
        int Length(string address);
    }
}
