using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PersistentQueueConfiguration : IPersistentQueueConfiguration
    {
        private string configurationString;

        public PersistentQueueConfiguration(string configurationString = "")
        {
            this.configurationString = configurationString;
        }

        public string ConfigurationString 
        { 
            get
            {
                return configurationString;
            }
            set
            {
                this.configurationString = value;
            }
        }
    }
}
