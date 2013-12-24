using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Identifies a user identity available for the user to operate as
    /// </summary>
    public class Identity
    {
        private string identifier;
        private string description;

        public Identity(string identifier, string description)
        {
            this.identifier = identifier;
            this.description = description;
        }

        public string Identifier
        {
            get
            {
                return identifier;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }
    }
}
