using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Entities;
using MercurioAppServiceLayer;

namespace Mercurio
{
    /// <summary>
    /// Transforms underlying Entities into ViewModel objects that WPF can DataBind and use
    /// to store UI state.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<UserViewModel> users, availableIdentities;
        private ObservableCollection<MessageViewModel> messages;
        private AppServiceLayer appService;
        private UserViewModel selectedUser = null, selectedIdentity = null;
        private bool locked = true, invitationPanelVisible = false;
        private int invitationPanelHeight = 0, invitationPanelExpandedHeight = 200;

        public MainWindowViewModel(AppServiceLayer appService)
        {
            this.appService = appService;
            this.appService.NewMessageEvent += NewMessage;

            users = new ObservableCollection<UserViewModel>(
                (from user in appService.GetUsers()
                 select new UserViewModel(user))
                 .ToList());

            availableIdentities = new ObservableCollection<UserViewModel>(
                (from user in appService.GetAvailableIdentities()
                 select new UserViewModel(user))
                 .ToList());

            if (availableIdentities.Count > 0)
                SelectedIdentity = availableIdentities[0];

            if (users.Count > 0)
                SelectedUser = users[0];

            this.Unlock = new UnlockCommand(this);
            this.ToggleInvitationPanel = new TogglePanelCommand(this);
        }

        #region Observable Properties
        public UserViewModel SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                if (value != selectedUser)
                {
                    selectedUser = value;
                    if (selectedUser != null)
                    {
                        Messages = new ObservableCollection<MessageViewModel>(
                            (from message in appService.GetMessages(selectedUser.Address)
                             select new MessageViewModel(message))
                            .ToList());
                    }
                    RaisePropertyChangedEvent("SelectedUser");
                }
            }
        }

        public UserViewModel SelectedIdentity
        {
            get
            {
                return selectedIdentity;
            }
            set
            {
                if (value != selectedIdentity)
                {
                    selectedIdentity = value;
                    RaisePropertyChangedEvent("SelectedIdentity");
                }
            }
        }

        public ObservableCollection<UserViewModel> Users
        {
            get
            {
                return users;
            }
            set
            {
                if (value != users)
                {
                    users = value;
                    RaisePropertyChangedEvent("Users");
                }
            }
        }

        public ObservableCollection<UserViewModel> AvailableIdentities
        {
            get
            {
                return availableIdentities;
            }
            set
            {
                if (value != availableIdentities)
                {
                    availableIdentities = value;
                    RaisePropertyChangedEvent("Identities");
                }
            }
        }

        public ObservableCollection<MessageViewModel> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                if (value != messages)
                {
                    messages = value;
                    RaisePropertyChangedEvent("Messages");
                }
            }
        }

        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                if (value != locked)
                {
                    locked = value;
                    RaisePropertyChangedEvent("Locked");
                }
            }
        }

        public int InvitationPanelHeight
        {
            get
            {
                return invitationPanelHeight;
            }
            set
            {
                if (value != invitationPanelHeight)
                {
                    invitationPanelHeight = value;
                    RaisePropertyChangedEvent("InvitationPanelHeight");
                }
            }
        }

        public bool InvitationPanelVisible
        {
            get
            {
                return invitationPanelVisible;
            }
            set
            {
                if (value != invitationPanelVisible)
                {
                    invitationPanelVisible = value;
                    RaisePropertyChangedEvent("InvitationPanelVisible");
                }
            }
        }

        public string FingerprintText
        {
            get
            {
                return appService.GetFingerprint(selectedIdentity.Identifier);
            }
        }
        #endregion

        public void NewMessage(IMercurioMessage message, string senderAddress)
        {
            UserViewModel user = users.FirstOrDefault<UserViewModel>(s => s.Address == senderAddress);

            if (user != null && user != selectedUser)
            {
                user.NumberOfUnreadMessages++;
                //messageCount++;
                //BadgeControl badgeControl = new BadgeControl();
                //badgeControl.SetText(messageCount.ToString());

                //OverlayAdorner<BadgeControl>.Overlay(dgMessages, badgeControl);                
            }
        }        

        #region Commands

        public ICommand Unlock { get; set; }
        public ICommand ToggleInvitationPanel { get; set; }

        #endregion

        public bool ValidatePassword(SecureString password)
        {
            Locked = false;
            return true; // TODO: Faked for now, connect up, see http://stackoverflow.com/questions/11381123/how-to-use-gpg-command-line-to-check-passphrase-is-correct
        }

        public void DoToggleInvitationPanel()
        { 
            InvitationPanelVisible = !InvitationPanelVisible;
            InvitationPanelHeight = invitationPanelVisible ? invitationPanelExpandedHeight : 0;
        }

        public void StartListener(SecureString password)
        {
            //appService.StartListener(selectedIdentity.Address, password.
        }
    }
}
