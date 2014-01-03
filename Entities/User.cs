using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User
    {
        private string identifier;
        private string description;
        private string address;
        private string name;

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

        public string Address
        {
            get
            {
                return address;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public User(string identifier, string name, string address, string description)
        {
            this.identifier = identifier;
            this.address = address;
            this.description = description;
            this.name = name;
        }
    }
}
