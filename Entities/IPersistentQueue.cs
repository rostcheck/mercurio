using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IPersistentQueue
    {
        void Add(IMercurioMessage message);
        IMercurioMessage GetNext(string address);
        int Length(string address);
    }
}
