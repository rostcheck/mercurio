using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class MercurioShellContext
    {
        public IMercurioEnvironment Environment { get; set; }
        public List<IExecutableMercurioCommand> Commands { get; set; }
        public Func<string, IMercurioEnvironment, bool> ConfirmAction { get; set; }
        public IContainer OpenContainer { get; set; }
    }
}
