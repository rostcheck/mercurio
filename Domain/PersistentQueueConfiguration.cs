using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercurio.Domain;

namespace Mercurio.Domain
{
    public class PersistentQueueConfiguration
    {
        private string _configurationString;
		private string _serviceType;
		private string _name;

        public PersistentQueueConfiguration(string name, string serviceType, string configurationString = "")
        {
            this._configurationString = configurationString;
			this._name = name;
			this._serviceType = serviceType;
        }

		public string ConfigurationString
        { 
            get
            {
                return _configurationString;
            }
            set
            {
                this._configurationString = value;
            }
        }

		public string Name
		{ 
			get
			{
				return _name;
			} 

			set
			{
				_name = value;
			}
		}

		public string ServiceType
		{
			get
			{
				return _serviceType;
			}
			set
			{
				_serviceType = value;
			}
		}
    }
}
