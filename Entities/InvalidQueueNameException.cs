using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class InvalidQueueNameException : MercurioException
    {
        public InvalidQueueNameException(string message)
            : base(message)
        {
        }
    }
}
