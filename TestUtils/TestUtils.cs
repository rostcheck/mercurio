using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            CopyGPGFiles(userSourceDir, KeyChainDirectory);           
        }

        public static void SwitchUser(string fromUser, string toUser)
        {
            if (fromUser != null && fromUser != string.Empty)
            {
                CopyGPGFiles(KeyChainDirectory, GetUserWorkingDir(fromUser));
            }
            CopyGPGFiles(GetUserWorkingDir(toUser), KeyChainDirectory);
        }

        private static void CopyGPGFiles(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            CopyFile("pubring.gpg", sourceDir, destinationDir);
            CopyFile("secring.gpg", sourceDir, destinationDir);
            CopyFile("trustdb.gpg", sourceDir, destinationDir);
        }

        private static void CopyFile(string fileName, string sourceDirectory, string destinationDirectory)
        {
            string sourcePath = sourceDirectory + "\\" + fileName;
            string destinationPath = destinationDirectory + "\\" + fileName;
            File.Copy(sourcePath, destinationPath, true); // overwrite
        }

        //public static void SetupDirs(List<string> userNames)
        //{
        //    if (!Directory.Exists(KeyChainDirectory))
        //    {
        //        Directory.CreateDirectory(KeyChainDirectory);
        //    }
        //    foreach (string userName in userNames)
        //    {
        //        SetupUserDir(userName);
        //    }
        //}
    }
}
