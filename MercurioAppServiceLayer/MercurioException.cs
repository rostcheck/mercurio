using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioAppServiceLayer
{
    public class MercurioException : Exception
    {
        public MercurioException(string message)
            : base(message)
        {
        }
    }
}
