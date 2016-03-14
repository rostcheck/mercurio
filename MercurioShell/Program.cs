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
    /// MercurioShell allows the user to interactively manipulate a Mercurio environment from the command line 
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
			var console = new MercurioConsole();
            bool exit = false;		
			bool newLine = false;
			var keys = new List<ConsoleKeyInfo>();

			Console.BackgroundColor = ConsoleColor.Black;
			console.WriteToConsole("");
			console.ResetCommandLine();
            Console.WriteLine("Welcome to Project Mercurio. Type 'help' for help or 'quit' to exit.");

			do
			{	// Process until exit
				do
				{	// Get a line
					var consoleKeyInfo = Console.ReadKey();
					switch(consoleKeyInfo.Key)
					{
						case ConsoleKey.Enter:
							newLine = true;
							break;
						case ConsoleKey.Backspace:
						case ConsoleKey.Delete:
							keys.RemoveAt(keys.Count - 1);
							break;
						case ConsoleKey.UpArrow:
							keys.Clear();
							AddToKeysRange(keys, console.BackHistory());
							console.ResetCommandLine(string.Join("", keys.Select(s => s.KeyChar).ToList()));
							break;
						case ConsoleKey.DownArrow:
							keys.Clear();
							AddToKeysRange(keys, console.ForwardHistory());
							console.ResetCommandLine(string.Join("", keys.Select(s => s.KeyChar).ToList()));
							break;
						default:
							keys.Add(consoleKeyInfo);
							break;
					}							
//					var left = Console.CursorLeft;
//					Console.SetCursorPosition(left, WriterRow);
				}  while (newLine == false);
				var line = string.Join("", keys.Select(s => s.KeyChar).ToList());
//				var lineLower = line.ToLower();
//				if (lineLower == "exit" || lineLower == "quit")
//					exit = true;
				
//                Console.Write("> ");
//                var line = Console.ReadLine();
                switch (line.ToLower().Trim())
                {
                    case "quit":
                    case "exit":
                        console.WriteToConsole("Goodbye");
                        exit = true;
                        break;
                    default:
                        try
                        {
							console.WriteToConsole(line);
							console.PushCommandLine(keys);

                            var results = shell.ExecuteCommand(line);
                            if (results != null)
                            {
								console.WriteToConsole(results);
                            }
                        }
                        catch (Exception ex)
                        {
							console.WriteToConsole(ex.Message);
                        }
                        break;
                }
				// Write to console
				//console.WriteToConsole(line);
				newLine = false; 
				keys.Clear(); // Clear buffers
				console.ResetCommandLine();
				console.ResetHistory();
            } while (!exit);
        }

		private static void AddToKeysRange(List<ConsoleKeyInfo> keys, List<ConsoleKeyInfo> newKeys)
		{
			if (keys != null && newKeys != null)
				keys.AddRange(newKeys);
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
