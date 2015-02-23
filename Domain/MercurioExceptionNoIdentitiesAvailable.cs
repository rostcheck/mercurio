using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercurio.Domain
{
    public class MercurioExceptionNoIdentitiesAvailable : MercurioException
    {
        public MercurioExceptionNoIdentitiesAvailable(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
