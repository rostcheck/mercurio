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
        const string ServiceTypeKey = "ServiceType";
        const string NameKey = "Name";
        const string ConfigurationStringKey = "ConfigurationString";

        public PersistentQueueConfiguration(string name, string serviceType, string configurationString = "")
        {
            this._configurationString = configurationString;
			this._name = name;
			this._serviceType = serviceType;
        }

        public PersistentQueueConfiguration(Dictionary<string, string> dictionary)
        {
            this._configurationString = dictionary[ConfigurationStringKey];
            this._name = dictionary[NameKey];
            this._serviceType = dictionary[ServiceTypeKey];
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

        public Dictionary<string, string> AsDictionary()
        {
            var dictionary = new Dictionary<string, string>();
            dictionary[ServiceTypeKey] = _serviceType;
            dictionary[NameKey] = _name;
            dictionary[ConfigurationStringKey] = _configurationString;
            return dictionary;
        }
    }
}
