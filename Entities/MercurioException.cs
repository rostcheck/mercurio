using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    ///  Base class for exceptions generated within Mercurio application logic (vs. system exceptions)
    /// </summary>
    public class MercurioException : Exception
    {
        public MercurioException(string message)
            : base(message)
        {
        }
    }
}
