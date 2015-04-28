using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilities
{
    public static class TestUtils
    {
        private const string TestKeyChainSourceDirectory = "..\\..\\..\\TestKeyRings";
        public static readonly string KeyChainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\gnupg";
        private static readonly string KeyChainBackupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\gnupg\\test-backup";
        private static readonly string KeyChainWorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TestKeyChains";
        private static readonly string TestKeyChainBaseDirectory = KeyChainWorkingDirectory;

        public static string GetUserDir(string userName)
        {
            return TestKeyChainBaseDirectory + "\\" + userName;
        }

        public static string GetUserWorkingDir(string userName)
        {
            return KeyChainWorkingDirectory + "\\" + userName;
        }

        public static string GetUserSourceDir(string userName)
        {
            return TestKeyChainSourceDirectory + "\\" + userName;
        }

        // Copies the keyrings from the test source dir to a working dir
        public static void SetupUserDir(string userName)
        {
            string userSourceDir = GetUserSourceDir(userName);
            string userWorkingDir = GetUserWorkingDir(userName);

            CopyGPGFiles(KeyChainDirectory, KeyChainBackupDirectory);
            CopyGPGFiles(userSourceDir, userWorkingDir);
        }

        public static void SwitchUser(string fromUser, string toUser)
        {
        }

        private static void CopyGPGFiles(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            CopyFileIfExists("pubring.gpg", sourceDir, destinationDir);
            CopyFileIfExists("secring.gpg", sourceDir, destinationDir);
            CopyFileIfExists("trustdb.gpg", sourceDir, destinationDir);
        }

        private static void CopyFileIfExists(string fileName, string sourceDirectory, string destinationDirectory)
        {
            string sourcePath = sourceDirectory + "\\" + fileName;
            if (File.Exists(sourcePath))
            {

                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);
                string destinationPath = destinationDirectory + "\\" + fileName;
                File.Copy(sourcePath, destinationPath, true); // overwrite
            }
        }

        public static void CleanupSubstrate(string storageDir)
        {
            if (storageDir != null)
            {
                foreach (var directory in Directory.EnumerateDirectories(storageDir, "*.*", SearchOption.AllDirectories))
                {
                    Directory.Delete(directory, true);
                }
                foreach (var file in Directory.EnumerateFiles(storageDir, "*.*", SearchOption.AllDirectories))
                {
                    File.Delete(file);
                }

            }
        }

        public static NetworkCredential PassphraseFunction(string identifier)
        {
            return new NetworkCredential(identifier, CryptoTestConstants.HermesPassphrase);
        }
    }
}
