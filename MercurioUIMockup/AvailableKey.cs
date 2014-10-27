using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioUIMockup
{
    public class AvailableKey : ViewModelBase
    {
        private string name;
        public AvailableKey(string name)
        {
            this.name = name;
        }

        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        public string ToString()
        {
            return name;
        }
    }
}
