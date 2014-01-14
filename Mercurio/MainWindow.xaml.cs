using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private bool exiting;
        private const int messagePoolTimeInMS = 1000;
        private const string title = "Mercurio Secure Communicator";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Setup();
            // Start the message receiving thread

            this.Title = title + "    User: " + appServiceLayer.GetIdentity().Name;
            dgUsers.DataContext = appServiceLayer.GetUsers();

        }

        private void Setup()
        {
            appServiceLayer = new AppServiceLayer(AppCryptoManagerType.GPG, this);

        }

        private async Task ReceiveMessageLoop()
        {
            while (!exiting)
            {
                await Task.Delay(messagePoolTimeInMS);
            }
        }

        private void btnInvite_Click(object sender, RoutedEventArgs e)
        {
            Window inviteWindow = new InvitationWindow();
            inviteWindow.Owner = this;
            inviteWindow.ShowDialog();
        }
    }
}
