using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class MercurioShellSyntaxException : MercurioShellException
    {
        public MercurioShellSyntaxException(string message) : base(message)
        {
        }
    }
}
