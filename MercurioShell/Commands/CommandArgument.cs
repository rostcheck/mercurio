using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    public class CommandArgument
    {
        public bool Required { get; private set; }
        public string Name { get; private set; }
        public string ArgumentNameForHelp { get; private set; }
        public string[] AllowedValues { get; private set; }

        public CommandArgument(string name, bool required, string argumentNameForHelp, string[] allowedValues = null)
        {
            this.Name = name;
            this.Required = required;
            this.ArgumentNameForHelp = argumentNameForHelp;
            this.AllowedValues = allowedValues;
        }
    }
}
