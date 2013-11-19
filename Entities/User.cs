using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User
    {
        public string Identifier
        {
            get
            {
                return identifier;
            }
        }

        private string identifier;

        public User(string identifier)
        {
            this.identifier = identifier;
        }
    }
}
