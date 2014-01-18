﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Entities;
using MercurioAppServiceLayer;

namespace Mercurio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMercurioUI
    {
        private AppServiceLayer appServiceLayer;
        //private bool exiting;
        private const int messagePoolTimeInMS = 1000;
        private const string title = "Mercurio Secure Communicator";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Setup();
            cmbOperatingUser.ItemsSource = appServiceLayer.GetAvailableIdentities();
            cmbOperatingUser.DisplayMemberPath = "Name";
            if (cmbOperatingUser.Items.Count > 0)
            {
                cmbOperatingUser.SelectedIndex = 0;
            }
            dgUsers.DataContext = appServiceLayer.GetUsers();
            User operatingUser = cmbOperatingUser.SelectedItem as User;
            if (operatingUser != null && dgUsers.DataContext != null)
            {
                foreach (User thisUser in (List<User>)dgUsers.DataContext)
                {
                    if (thisUser.Address != operatingUser.Address)
                    {
                        dgUsers.SelectedItem = thisUser;
                        break;
                    }
                }
            }
            await InjectTestMessages();
        }

        private void Setup()
        {
            appServiceLayer = new AppServiceLayer(AppCryptoManagerType.GPG, this);

        }

        private void btnInvite_Click(object sender, RoutedEventArgs e)
        {
            Window inviteWindow = new InvitationWindow();
            inviteWindow.Owner = this;
            inviteWindow.ShowDialog();
        }

        public void NewMessage(IMercurioMessage message, string senderAddress)
        {
            User selectedUser = dgUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                dgMessages.DataContext = appServiceLayer.GetMessages(selectedUser.Address);
            }
        }

        public string GetSelectedIdentity(ICryptoManager cryptoManager)
        {
            User selectedIdentity = cmbOperatingUser.SelectedItem as User;
            if (selectedIdentity != null)
            {
                return selectedIdentity.Identifier;
            }
            else
            {
                return null;
            }
        }

        public bool AcceptInvitation(ConnectInvitationMessage invitationMessage, string fingerprint)
        {
            throw new NotImplementedException();
        }

        public bool AcceptInvitationResponse(ConnectInvitationAcceptedMessage invitationAcceptedMessage, string fingerprint)
        {
            throw new NotImplementedException();
        }

        public void InvalidMessageReceived(object message)
        {
            tbStatus.Text = "Invalid message received (see log)";
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User selectedUser = dgUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                dgMessages.DataContext = appServiceLayer.GetMessages(selectedUser.Address);
            }
        }

        private void OnOperatingUserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User operatingUser = cmbOperatingUser.SelectedItem as User;
            if (operatingUser != null)
            {
                this.Title = title + "    User: " + operatingUser.Name;
            }
        }

        private async Task InjectTestMessages()
        {
            const int delay = 5000;
            const string sender = "bob@maker.net";
            const string recipient = "alice@maker.net";
            string[] messages = {"Hi Alice, are you there?", "I'm Bob, how are you?", "Are you going to the party on Saturday?",
                                "I'm not bothering you, am I?", "I'm going, I think it will be fun."};
            foreach (string messageText in messages)
            {
                await Task.Delay(delay);
                SimpleTextMessage message = new SimpleTextMessage(sender, recipient, messageText);
                appServiceLayer.DisplayMessage(message, sender);
            }
        }
    }
}
