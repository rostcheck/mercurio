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
            var shell = new MercurioCommandShell(environment);
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
            var environment = MercurioEnvironment.Create(environmentScanner, serializer, GetLoginInfo);
           // environment.SetUserHomeDirectory(userHome);
            var identities = environment.GetAvailableIdentities();
            Console.WriteLine("Available identities are:");
            foreach(var identity in identities)
            {
                Console.WriteLine(identity.Name);
            }
            Console.WriteLine("Enter desired identity or 'quit' to exit: ");
            Console.Write("> ");
            bool done = false;
            bool success = false;
            do
            {
                var identityName = Console.ReadLine().Trim();
                var identityNameLowercase = identityName.ToLower();
                if (identityNameLowercase == "quit")
                {
                    done = true;
                }
                else
                {
                    var selectedIdentity = identities.Where(s => s.Name.Trim().ToLower() == identityNameLowercase).FirstOrDefault();
                    if (selectedIdentity == null)
                    {
                        Console.WriteLine(string.Format("Identity {0} does not exist. Please choose from the list above.", identityName));
                        Console.Write("> ");
                        continue;
                    }
                    try
                    {
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
