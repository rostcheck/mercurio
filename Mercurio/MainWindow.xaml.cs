using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MercurioUIControls;

namespace Mercurio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml. Thin UI; should only do presentation.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int messagePoolTimeInMS = 1000;
        private const string title = "Mercurio Secure Communicator";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            //TODO: Ask for passphrase in dialog
            string passphrase = @"Of all the queens, Alice is the highest!";
            string selectedAddress = GetSelectedAddress();
            //await appServiceLayer.StartListener(selectedAddress, passphrase);
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

        public string GetSelectedAddress()
        {
            User selectedIdentity = cmbOperatingUser.SelectedItem as User;
            if (selectedIdentity != null)
            {
                return selectedIdentity.Address;
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
            User currentUser = cmbOperatingUser.SelectedItem as User;
            if (selectedUser != null)
            {
                //dgMessages.DataContext = appServiceLayer.GetMessages(selectedUser.Address);
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

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //appServiceLayer.StopListener();
        }
    }
}
