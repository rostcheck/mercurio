using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell
{
    /// <summary>
    /// MercurioShell allows the user to manipulate a Mercurio environment from the command line 
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            var environment = Setup();
            if (environment == null)
            {
                Console.WriteLine("Unable to set up a Mercurio environment. Goodbye.");
                return;
            }
            var shell = MercurioCommandShell.Create(environment, ConfirmAction);
            bool exit = false;
            Console.WriteLine("Welcome to Project Mercurio. Type 'help' for help or 'quit' to exit.");
            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                switch (line.ToLower().Trim())
                {
                    case "quit":
                    case "exit":
                        Console.WriteLine("Goodbye");
                        exit = true;
                        break;
                    default:
                        try
                        {
                            var results = shell.ExecuteCommand(line);
                            if (results != null)
                            {
                                foreach (var result in results)
                                {
                                    Console.WriteLine(result);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                }
            } while (!exit);
        }

        private static IMercurioEnvironment Setup()
        {
            var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var environmentScanner = new EnvironmentScanner(userHome);
            var storageSubstrates = environmentScanner.GetStorageSubstrates();
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            var environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, GetLoginInfo);
            // environment.SetUserHomeDirectory(userHome);
            var identities = environment.GetAvailableIdentities();
            Console.WriteLine("Available identities are:");
			for (int identityCounter = 0; identityCounter < identities.Count; identityCounter++)
            {
				var identity = identities[identityCounter];
				string expirationDate = identity.ExpirationDate.HasValue ?  string.Format(" (expires {0})", identity.ExpirationDate.Value.ToShortDateString())									
					: "";
				Console.WriteLine(string.Format("{0}] {1} ({2}): {3} {4}", identityCounter, identity.Name, identity.Address, identity.Description, 
					expirationDate));
            }
            bool done = false;
            bool success = false;
            do
            {
                Console.WriteLine("Enter desired identity or 'quit' to exit: ");
                Console.Write("> ");
                var identityNumberText = Console.ReadLine().Trim();
				var identityNumberLowercase = identityNumberText.ToLower();
                if (identityNumberLowercase == "quit")
                {
                    done = true;
                }
                else
                {
					var identityNumber = Convert.ToInt32(identityNumberText);
					if (identityNumber < 0 || identityNumber >= identities.Count)
					{
						Console.WriteLine(string.Format("'{0}' is not a valid choice. Please choose from the list above.", identityNumber));
						Console.Write("> ");
						continue;						
					}
					var selectedIdentity = identities[identityNumber];
                    try
                    {
						Console.WriteLine("Trying to set active environment to " + selectedIdentity.UniqueIdentifier + " " + selectedIdentity.Name + " " + selectedIdentity.Description + " " + selectedIdentity.Address);
                        environment.SetActiveIdentity(selectedIdentity);
                        success = true;
                        done = true;
                    }
                    catch (MercurioException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            } while (done == false);
            return (success == true) ? environment : null;
        }

        private static bool ConfirmAction(string prompt, IMercurioEnvironment environment)
        {
            var activeIdentity = environment.GetActiveIdentity();
            if (activeIdentity == null)
            {
                Console.WriteLine("Active identity is not set");
                return false;
            }

            Console.WriteLine(prompt);
            Console.WriteLine(string.Format("Current identity is {0}. Enter passphrase to confirm action:", activeIdentity.Name));
            return environment.ConfirmActiveIdentity();
        }

        private static NetworkCredential GetLoginInfo(string userName)
        {
            Console.Write("Passphrase: ");
            SecureString passPhrase = new SecureString();
            var c = Console.ReadKey(true);
            while (c.Key != ConsoleKey.Enter)
            {
                passPhrase.AppendChar(c.KeyChar);
                c = Console.ReadKey(true);
            }
            Console.WriteLine();
            return new NetworkCredential(userName, passPhrase);
        }
    }
}
