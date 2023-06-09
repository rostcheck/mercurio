﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Resources;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Entities;
using MercurioAppServiceLayer;
using Mercurio.Properties;
using System.Windows;

namespace Mercurio
{
    /// <summary>
    /// Transforms underlying Entities into ViewModel objects that WPF can DataBind and use
    /// to store UI state.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<IdentityViewModel> users, availableIdentities;
        private ObservableCollection<MessageViewModel> messages;
        private ObservableCollection<ConnectInvitationMessageViewModel> invitations;
        private AppServiceLayer appService;
        private IdentityViewModel selectedUser = null, selectedIdentity = null;
        private bool locked = true, invitationPanelVisible = false, passwordInvalid = false;
        private int invitationPanelHeight = 0, invitationPanelExpandedHeight = 200;
        private NetworkCredential credential;
        private string evidenceURL, receipientAddress, messageToSend;
        private ConnectInvitationMessageViewModel selectedInvitation;
       
        public MainWindowViewModel(AppServiceLayer appService)
        {
            this.appService = appService;
            this.appService.NewMessageEvent += NewMessage;
            this.appService.ReplacedMessageEvent += ReplacedMessage;
            this.appService.NewInvitationEvent += NewInvitation;

            users = new ObservableCollection<IdentityViewModel>(
                (from contact in appService.GetContacts()
                 select new IdentityViewModel(contact))
                 .ToList());

            availableIdentities = new ObservableCollection<IdentityViewModel>(
                (from user in appService.GetAvailableIdentities()
                 select new IdentityViewModel(user))
                 .ToList());

            if (availableIdentities.Count > 0)
                SelectedIdentity = availableIdentities[0];

            if (users.Count > 0)
                SelectedUser = users[0];

            Invitations = new ObservableCollection<ConnectInvitationMessageViewModel>(
                (from invitation in appService.GetInvitations()
                 select new ConnectInvitationMessageViewModel(invitation, 
                    appService.GetFingerprint(invitation.KeyID)))
                .ToList());

            this.Unlock = new UnlockCommand(this);
            this.ToggleInvitationPanel = new TogglePanelCommand(this);
            this.SendInvitation = new SendInvitationCommand(this);
            this.AcceptInvitation = new AcceptInvitationCommand(this);
            this.RejectInvitation = new RejectInvitationCommand(this);
            this.SendMessage = new SendMessageCommand(this);
        }

        #region Observable Properties
        public IdentityViewModel SelectedUser
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
                        selectedUser.NumberOfUnreadMessages = 0;
                    }
                    RaisePropertyChangedEvent("SelectedUser");
                    RaisePropertyChangedEvent("MessageIsSendable");
                    SelectedInvitation = null; 
                }
            }
        }

        public ConnectInvitationMessageViewModel SelectedInvitation
        {
            get
            {
                return selectedInvitation;
            }
            set
            {
                if (value != selectedInvitation)
                {
                    selectedInvitation = value;
                    RaisePropertyChangedEvent("SelectedInvitation");
                    RaisePropertyChangedEvent("HasInvitationSelected");
                }
            }
        }

        public bool HasInvitationSelected
        {
            get
            {
                return (selectedInvitation != null);
            }
        }

        public bool MessageIsSendable
        {
            get
            {
                return (messageToSend != null && messageToSend.Length > 0 && selectedUser != null);
            }
        }

        public string MessageToSend
        {
            get
            {
                return messageToSend;
            }
            set
            {
                if (value != messageToSend)
                {
                    messageToSend = value;
                    RaisePropertyChangedEvent("MessageToSend");
                    RaisePropertyChangedEvent("MessageIsSendable");
                }
            }
        }

        public IdentityViewModel SelectedIdentity
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
                    appService.SetSelectedIdentity(value.Identifier);
                    RaisePropertyChangedEvent("SelectedIdentity");
                }
            }
        }

        public ObservableCollection<IdentityViewModel> Users
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

        public ObservableCollection<IdentityViewModel> AvailableIdentities
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

        public ObservableCollection<ConnectInvitationMessageViewModel> Invitations
        {
            get
            {
                return invitations;
            }
            set
            {
                if (value != invitations)
                {
                    invitations = value;
                    RaisePropertyChangedEvent("Invitations");
                    RaisePropertyChangedEvent("HasInvitations");
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

        public bool PasswordInvalid
        {
            get
            {
                return passwordInvalid;             
            }
            set
            {
                if (value != passwordInvalid)
                {
                    passwordInvalid = value;
                    RaisePropertyChangedEvent("PasswordInvalid");
                    RaisePropertyChangedEvent("PasswordValidationMessage");
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

        public string PasswordValidationMessage
        {
            get 
            {
                return passwordInvalid ? Resources.InvalidPasswordMessage : Resources.ProvidePasswordMessage;                    
            }
        }

        public string EvidenceURL
        {
            get
            {
                return evidenceURL;
            }
            set
            {
                if (value != evidenceURL)
                {
                    evidenceURL = value;
                    RaisePropertyChangedEvent("EvidenceURL");
                    RaisePropertyChangedEvent("CanSendInvitation");
                }
            }
        }

        public string RecipientAddress
        {
            get
            {
                return receipientAddress;
            }
            set
            {
                if (value != receipientAddress)
                {
                    receipientAddress = value;
                    RaisePropertyChangedEvent("RecipientAddress");
                    RaisePropertyChangedEvent("CanSendInvitation");
                }
            }
        }

        public bool CanSendInvitation
        {
            get
            {
                return (!String.IsNullOrEmpty(receipientAddress) && !String.IsNullOrEmpty(evidenceURL));
            }
        }

        public bool HasInvitations
        {
            get
            {
                return (invitations != null && invitations.Count > 0);
            }
        }
        #endregion

        #region Commands
        public ICommand Unlock { get; set; }
        public ICommand ToggleInvitationPanel { get; set; }
        public ICommand SendInvitation { get; set; }
        public ICommand AcceptInvitation { get; set; }
        public ICommand RejectInvitation { get; set; }
        public ICommand SendMessage { get; set; }
        #endregion

        #region Public Methods (hooked to AppService events)
        private void NewInvitation(ConnectInvitationMessage message, string senderAddress)
        {
            Invitations.Add(new ConnectInvitationMessageViewModel(message, appService.GetFingerprint(message.KeyID)));
            RaisePropertyChangedEvent("Invitations");
            RaisePropertyChangedEvent("HasInvitations");
        }

        public void NewMessage(IMercurioMessage message, string senderAddress)
        {
            IdentityViewModel user = users.FirstOrDefault<IdentityViewModel>(s => s.Address == senderAddress);

            if (user == null)
            {
                // message received from unknown user
            }
            else 
            {
                if (user == selectedUser)
                {
                    Messages.Add(new MessageViewModel(message));
                }
                else
                {
                    user.NumberOfUnreadMessages++;
                    //messageCount++;
                    //BadgeControl badgeControl = new BadgeControl();
                    //badgeControl.SetText(messageCount.ToString());

                    //OverlayAdorner<BadgeControl>.Overlay(dgMessages, badgeControl);                
                }
            }
        }

        public void ReplacedMessage(IMercurioMessage message, string senderAddress)
        {
            IdentityViewModel user = users.FirstOrDefault<IdentityViewModel>(s => s.Address == senderAddress);

            if (user == null)
            {
                // message received from unknown user
            }
            else
            {
                if (user == selectedUser)
                {
                    var thisMessage = Messages.FirstOrDefault<MessageViewModel>(s => s.MessageID == message.ContentID);
                    if (thisMessage != null)
                    {
                        Messages[Messages.IndexOf(thisMessage)] = new MessageViewModel(message);
                    }
                }
            }
        }
        #endregion

        #region Public Methods (called by commands)
        public bool ValidatePassword(SecureString password)
        {
            credential = new NetworkCredential(selectedIdentity.Identifier, password);
            bool isValid = appService.ValidateCredential(credential);
            Locked = !isValid;
            return isValid;
        }

        public void DoToggleInvitationPanel()
        { 
            InvitationPanelVisible = !InvitationPanelVisible;
            InvitationPanelHeight = invitationPanelVisible ? invitationPanelExpandedHeight : 0;
        }

        public void StartListener()
        {
            if (credential == null)
                throw new Exception("Cannot start listener without having a password set");

            appService.StartListener(selectedIdentity.Address, credential);
        }

        public void DoSendInvitation()
        {
            appService.SendInvitation(selectedIdentity.Identifier, selectedIdentity.Address, receipientAddress, evidenceURL);
            RecipientAddress = string.Empty;
            EvidenceURL = string.Empty;
            DoToggleInvitationPanel();
        }

        public void DoAcceptInvitation()
        {
            appService.AcceptInvitation(selectedIdentity.Identifier, selectedInvitation.Message);
            Invitations.Remove(selectedInvitation);
            SelectedInvitation = null;
            RaisePropertyChangedEvent("Invitations");
            RaisePropertyChangedEvent("HasInvitations");
        }

        public void DoRejectInvitation()
        {
            appService.RejectInvitation(selectedInvitation.Message);
            Invitations.Remove(selectedInvitation);
            SelectedInvitation = null;
            RaisePropertyChangedEvent("Invitations");
            RaisePropertyChangedEvent("HasInvitations");
        }

        public void DoSendMessage()
        {
            appService.SendMessage(selectedIdentity.Address, selectedUser.Address, messageToSend);
            MessageToSend = string.Empty;
        }
        #endregion
    }
}
