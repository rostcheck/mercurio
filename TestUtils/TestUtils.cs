using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtils
{
    public static class TestUtils
    {
        private const string TestKeyChainSourceDirectory = "..\\..\\..\\TestKeyRings";
        private static readonly string KeyChainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\gnupg";
        private static readonly string KeyChainBackupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\gnupg\\test-backup";

        public static string GetUserDir(string userName)
        {
            return KeyChainDirectory;
        }

        public static string GetUserSourceDir(string userName)
        {
            return TestKeyChainSourceDirectory + "\\" + userName;
        }

        public static string SetupUserDir(string userName)
        {
            string userSourceDir = GetUserSourceDir(userName);
            string userDestDir = GetUserDir(userName);

            CopyGPGFiles(KeyChainDirectory, KeyChainBackupDirectory);
            CopyGPGFiles(userSourceDir, KeyChainDirectory);
            return userDestDir;
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
