using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Mercurio
{
    public class UserViewModel : ViewModelBase
    {
        private User user;
        private int numberOfUnreadMessages;

        public UserViewModel(User user)
        {
            this.user = user;
        }

        public string Name
        {
            get
            {
                return user.Name;
            }
        }

        public string Address
        {
            get
            {
                return user.Address;
            }
        }

        public string Identifier
        {
            get
            {
                return user.Identifier;
            }
        }

        public int NumberOfUnreadMessages
        {
            get
            {
                return numberOfUnreadMessages;
            }
            set
            {
                numberOfUnreadMessages = value;
                RaisePropertyChangedEvent("NumberOfUnreadMessage");
                RaisePropertyChangedEvent("NameAndNumberUnread");
            }
        }

        public string NameAndNumberUnread
        {
            get
            {
                return numberOfUnreadMessages > 0 ? Name + " (" + NumberOfUnreadMessages + ")" : Name;
            }
        }
    }
}
