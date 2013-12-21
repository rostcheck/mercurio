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
        private static readonly string TestKeyChainBaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TestKeychains";

        public static string GetUserDir(string userName)
        {
            return TestKeyChainBaseDirectory + "\\" + userName;
        }

        public static string GetUserSourceDir(string userName)
        {
            return TestKeyChainSourceDirectory + "\\" + userName;
        }

        public static string SetupUserDir(string userName)
        {
            string userSourceDir = GetUserSourceDir(userName);
            string userDestDir = GetUserDir(userName);

            if (!Directory.Exists(userDestDir))
            {
                Directory.CreateDirectory(userDestDir);
            }
            CopyFile("pubring.gpg", userSourceDir, userDestDir);
            CopyFile("secring.gpg", userSourceDir, userDestDir);
            CopyFile("trustdb.gpg", userSourceDir, userDestDir);
            return userDestDir;
        }

        private static void CopyFile(string fileName, string sourceDirectory, string destinationDirectory)
        {
            string sourcePath = sourceDirectory + "\\" + fileName;
            string destinationPath = destinationDirectory + "\\" + fileName;
            File.Copy(sourcePath, destinationPath, true); // overwrite
        }

        public static void SetupDirs(List<string> userNames)
        {
            if (!Directory.Exists(TestKeyChainBaseDirectory))
            {
                Directory.CreateDirectory(TestKeyChainBaseDirectory);
            }
            foreach (string userName in userNames)
            {
                SetupUserDir(userName);
            }
        }
    }
}
