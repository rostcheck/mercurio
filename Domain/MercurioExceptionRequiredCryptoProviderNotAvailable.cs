using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Exception thrown when an operation requires a crypto provider that is not available
    /// </summary>
    public class MercurioExceptionRequiredCryptoProviderNotAvailable : MercurioException
    {
        public MercurioExceptionRequiredCryptoProviderNotAvailable(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
