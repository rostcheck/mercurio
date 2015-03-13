using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Starksoft.Cryptography.OpenPGP;
using TestUtilities;

namespace TestCryptography
{
    [TestClass]
    public class GnuPGTest
    {
        private const string hermesPublicKeyID = "6C628193";
        private const string hermesPublicSubKeyID = "44AE5384";
        private const string hermesPassphrase = @"Our technology has been TurneD AGAINST US :(";
        private const string secretMessage = "This is my secret message. There are many like it, but this one is mine.";
        // Test key passphrase: This$is$the$test$key
        private const string testPublicKeyID = "3EF9BE07";
        private const string testPublicKeyFingerprint = "5037 6136 157B 7E02 D26E  D2AF 00DB 8CA0 3EF9 BE07";
        private const string testPublicKey = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: GnuPG v2

mI0EVGV2BgEEAKP9Yx2KIa5/6ro38/caw8UoiatrCmHJ+tDeRa0ibsYAafYQ6gIj
myhxA7xe8k7ZEddZmwlKe4jCehC9oSCFWLdgrXRg0CI6SfrOZxj5m7MT6QtUI5op
sG6ElB/ZZte0/DbUNJncRgws1cG6gnoZicf76H9/WSQZ+zANBSVgTvypABEBAAG0
OVRlc3QgS2V5IChHUEcga2V5IGZvciBNZXJjdXJpbyB1bml0IHRlc3RzKSA8dGVz
dEBrZXkuY29tPoi5BBMBAgAjBQJUZXYGAhsDBwsJCAcDAgEGFQgCCQoLBBYCAwEC
HgECF4AACgkQANuMoD75vgd0tgP8CkUMvDoMt6gCMxJuDP5A3FXY9JsxPNQcUQyR
gg4aIQCRFJlNCVe3IOAqdhJYlqc9+QajSffMtDzZyyFEL2h5WPR94m7CVo0HOiIp
jEHW99NhMgj/TV0RVdWWhHGz6HRlINxDqxHh36+EQHhpKdmkqvedhr2Sl9O7goMx
AJIwHXO4jQRUZXYGAQQAy28wpX/+1hrqRqXYFpNxYnCaLnqFTDRw2Gas5CgTgnjT
+3SFCuDaVKHJIh7Z8Fmupcq6t6cyo7hLbhLvvW0OOEaQALLyReb0r0M+Piccmqk9
ldE1qI04q4Ecdh/0DwUMQ4kTVd65Pn5i7a0SLLMh0uwmcfH4j2vum111ug2nVq8A
EQEAAYifBBgBAgAJBQJUZXYGAhsMAAoJEADbjKA++b4He90EAJXEmgfdP4QARZ5O
kWEvYKqQbh7cZTqfLYehR03esM1M6BgdkiWzdB6RD56CjoEWt7D6tKS7xmYv7d+s
1J8Uug4lLSPYEnimAOnuBJDqSNdofB4w16PnQaqv6cCx6JZgmw45sgmoS/2ZGR34
dI/J+ka6IXbxS/1QzYPaRrGiaun0
=i5DD
-----END PGP PUBLIC KEY BLOCK-----";

        [TestMethod]
        public void ListKeysTest()
        {
            GnuPG gpg = PrepareTest("mercurio");
            GnuPGKeyCollection keys = gpg.GetKeys();
            Assert.IsTrue(keys.Count >= 2);
            GnuPGKey hermesKey = keys.FirstOrDefault<GnuPGKey>(s => s.KeyID == hermesPublicKeyID);
            Assert.IsNotNull(hermesKey);
            Assert.IsTrue(hermesKey.KeyLength == 4096);
            Assert.IsTrue(hermesKey.Algorithm == GnuPGAlgorithmType.RSA);
            Assert.IsTrue(hermesKey.CreationDate == new DateTime(2013, 12, 22));
            Assert.IsNull(hermesKey.KeyExpiration);
            Assert.IsTrue(hermesKey.SubKeys.Count == 1);
            GnuPGKey hermesSubKey = hermesKey.SubKeys[0];
            Assert.IsNotNull(hermesSubKey);
            Assert.IsTrue(hermesSubKey.KeyLength == 4096);
            Assert.IsTrue(hermesSubKey.Algorithm == GnuPGAlgorithmType.RSA);
            Assert.IsTrue(hermesSubKey.CreationDate == new DateTime(2013, 12, 22));
            Assert.IsNull(hermesSubKey.KeyExpiration);
            Assert.IsTrue(hermesSubKey.SubKeys.Count == 0);
            Assert.IsTrue(hermesSubKey.KeyID == hermesPublicSubKeyID);
        }

        private GnuPG PrepareTest(string userName)
        {
            TestUtils.SetupUserDir(userName);
            TestUtils.SwitchUser(null, userName);
            string keyPath = TestUtils.GetUserWorkingDir(userName);
            string binaryPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";
            return new GnuPG(keyPath, binaryPath);
        }

        [TestMethod]
        [ExpectedException(typeof(GnuPGException))]
        public void ImportBadKeyTest()
        {
            GnuPG gpg = PrepareTest("mercurio");
            string badKey = "This is not a valid key";
            gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(badKey)));
        }

        [TestMethod]
        public void ImportGoodKeyTest()
        {
            GnuPG gpg = PrepareTest("mercurio");
            NetworkCredential hermesCredential = new NetworkCredential(hermesPublicKeyID, hermesPassphrase);
            gpg.Credential = hermesCredential;

            // If the key is on the ring, delete it
            GnuPGKey key = gpg.GetKeys().FirstOrDefault(s => s.KeyID == testPublicKeyID);
            if (key != null)
            {
                gpg.DeleteKey(key.KeyID);
                // Verify it isn't there
                key = gpg.GetKeys().FirstOrDefault(s => s.KeyID == testPublicKeyID);
                Assert.IsNull(key);
            }

            gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(testPublicKey)));
            key = gpg.GetKeys().FirstOrDefault(s => s.KeyID == testPublicKeyID);
            Assert.IsNotNull(key);

            string fingerprint = gpg.GetFingerprint(key.KeyID);
            Assert.IsTrue(fingerprint == testPublicKeyFingerprint);

            // Sign the key
            gpg.SignKey(key.KeyID);

            string signedKey = gpg.GetActualKey(key.KeyID);
            Assert.IsTrue(signedKey.Length > 0);
        }

        [TestMethod]
        public void EncryptDecryptTest()
        {
            GnuPG gpg = PrepareTest("mercurio");
            NetworkCredential credential = new NetworkCredential(hermesPublicKeyID, hermesPassphrase);
            gpg.Recipient = hermesPublicKeyID; // Encrypt to ourself

            // verify encryption
            MemoryStream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(secretMessage));
            MemoryStream outputStream = new MemoryStream();
            MemoryStream metadataStream = new MemoryStream();
            gpg.Credential = credential;
            gpg.Encrypt(inputStream, outputStream, metadataStream);
            outputStream.Position = 0;
            StreamReader reader = new StreamReader(outputStream);
            string encryptedMessage = reader.ReadToEnd();
            Assert.IsTrue(encryptedMessage != null);
            Assert.IsTrue(encryptedMessage.Length > 0);

            // Verify decryption
            inputStream = new MemoryStream(Encoding.ASCII.GetBytes(encryptedMessage));
            outputStream = new MemoryStream();
            gpg.Decrypt(inputStream, outputStream, metadataStream);
            outputStream.Position = 0;
            reader = new StreamReader(outputStream);
            string decryptedMessage = reader.ReadToEnd();
            Assert.IsTrue(decryptedMessage == secretMessage);
        }
    }    
}