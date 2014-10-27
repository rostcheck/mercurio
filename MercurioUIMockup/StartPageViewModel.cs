using MercurioAppServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioUIMockup
{
    public class StartPageViewModel : ViewModelBase
    {
        const string importKeyText = "Import Existing Key";
        const string createKeyText = "Create New Key";
        private ObservableCollection<AvailableKey> availableKeys;

        private AppServiceLayer appServiceLayer;
        public StartPageViewModel(AppServiceLayer appServiceLayer)
        {
            this.appServiceLayer = appServiceLayer;
            var keyList = new ObservableCollection<AvailableKey>();
            keyList.Add(new AvailableKey("First Key"));
            keyList.Add(new AvailableKey("Second Key"));
            keyList.Add(new AvailableKey(importKeyText));
            keyList.Add(new AvailableKey(createKeyText));
            availableKeys = keyList;
        }

        public ObservableCollection<AvailableKey> AvailableKeys 
        { 
            get { 
                return availableKeys; 
            }
            set { availableKeys = value;  }
        }
    }
}
