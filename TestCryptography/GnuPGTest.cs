using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Starksoft.Cryptography.OpenPGP;

namespace TestCryptography
{
    [TestClass]
    public class GnuPGTest
    {
        [TestMethod]
        public void TestListKeys()
        {
            string keyPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\gnupg";
            string binaryPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";

            GnuPG gpg = new GnuPG(keyPath, binaryPath);
            GnuPGKeyCollection keys = gpg.GetKeys();
            Assert.IsTrue(keys.Count >= 2);
            GnuPGKey puttyKey = keys.FirstOrDefault<GnuPGKey>(s => s.Key == "B41CAE29");
            Assert.IsNotNull(puttyKey);
            Assert.IsTrue(puttyKey.KeyLength == 1024);
            Assert.IsTrue(puttyKey.Algorithm == GnuPGAlgorithmType.RSA);
            Assert.IsTrue(puttyKey.CreationDate == new DateTime(2000, 12, 20));
            Assert.IsNull(puttyKey.KeyExpiration);
            GnuPGKey hermesKey = keys.FirstOrDefault<GnuPGKey>(s => s.Key == "79222C24");
            Assert.IsNotNull(hermesKey);
            Assert.IsTrue(hermesKey.KeyLength == 4096);
            Assert.IsTrue(hermesKey.Algorithm == GnuPGAlgorithmType.RSA);
            Assert.IsTrue(hermesKey.CreationDate == new DateTime(2013, 11, 26));
            Assert.IsNull(hermesKey.KeyExpiration);
            Assert.IsTrue(hermesKey.SubKeys.Count == 1);
            GnuPGKey hermesSubKey = hermesKey.SubKeys[0];
            Assert.IsNotNull(hermesSubKey);
            Assert.IsTrue(hermesSubKey.KeyLength == 4096);
            Assert.IsTrue(hermesSubKey.Algorithm == GnuPGAlgorithmType.RSA);
            Assert.IsTrue(hermesSubKey.CreationDate == new DateTime(2013, 11, 26));
            Assert.IsNull(hermesSubKey.KeyExpiration);
            Assert.IsTrue(hermesSubKey.SubKeys.Count == 0);
            Assert.IsTrue(hermesSubKey.Key == "51649C30");
        }
    }
}
