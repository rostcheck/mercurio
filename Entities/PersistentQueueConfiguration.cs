using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;

namespace Entities
{
    public class PersistentQueueConfiguration : IPersistentQueueConfiguration
    {
        private string configurationString;
		private string name;

        public PersistentQueueConfiguration(string name, string configurationString = "")
        {
            this.configurationString = configurationString;
			this.name = name;
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

		public string Name
		{ 
			get
			{
				return name;
			} 

			set
			{
				name = value;
			}
		}
    }
}
