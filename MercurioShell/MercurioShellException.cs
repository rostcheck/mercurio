using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class MercurioShellException : Exception
    {
        public MercurioShellException(string message) : base(message)
        {
        }
    }
}
