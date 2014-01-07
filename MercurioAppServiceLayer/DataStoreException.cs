using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioAppServiceLayer
{
    public class DataStoreException : MercurioException
    {
        public DataStoreException(string message)
            : base(message)
        {
        }
    }
}
