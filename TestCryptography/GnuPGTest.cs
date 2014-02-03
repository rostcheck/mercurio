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
        private const string puttyPublicKeyID = "B41CAE29";
        private const string puttyPublicKeyFingerprint = "AE 65 D3 F7 85 D3 18 E0  3B 0C 9B 02 FF 3A 81 FE";
        private const string puttyPublicKey = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: GnuPG v2.0.22 (MingW32)

mQCNAzpA2ZYAAAEEAKxRyqIqZxKktdbrPo/OUj/4ij+yNIC8oBTVNgt3+NcAgFKI
lPxjRKkrNFMrmXPaKRLp8/TS5Z46nSgG44d58G/5clu7IVge2YlCpvoIfo3ute2U
UbBvXCJFVK5ePhNzQX8nGibmejxo8wF5CShyifhmoyfd96cf9u85zMC0HK4pAAUR
tDRQdVRUWSBSZWxlYXNlcyAoUlNBKSA8cHV0dHktYnVnc0BsaXN0cy50YXJ0YXJ1
cy5vcmc+iQCVAwUQOkDZlu85zMC0HK4pAQG7XQP6AlJiPCmN7PMz92LhqNbK4B1Y
WtNCcDKz1wZZow2OrQW79SZ+I6RqWC/z/1YKIgYY30aE2UtY9OMUYlQ+I08rsYC4
hNxNY4JvyyP9AT7wiJlpvdWtD590Z5DYwdSTeZg8w0N3NvCJ6m3ivFHiEZuJcZjd
cfPQziUxGjQi6HzG+seJAJUDBRA6QNrinVh3vx40rEEBAfM0A/9vCcf0Kj5ebQ6d
1oJmvOvp85jCy0Kwq4laatx8u9EhuKr0cSIFYLs64u3nwvTH2kRrlKRTqbdglA/D
yYBSdHwSzJ1LoQFARMVD7rxc8VIwNhZSwze3Tafp3iToiG/wTb6GE8rnPS+ExAja
LcrXlt1MbO1jFunrxKc3bwqez6ahvw==
=t5QN
-----END PGP PUBLIC KEY BLOCK-----";

        [TestMethod]
        public void ListKeysTest()
        {
            GnuPG gpg = PrepareTest("mercurio");
            GnuPGKeyCollection keys = gpg.GetKeys();
            Assert.IsTrue(keys.Count >= 2);
            GnuPGKey puttyKey = keys.FirstOrDefault<GnuPGKey>(s => s.KeyID == puttyPublicKeyID);
            Assert.IsNotNull(puttyKey);
            Assert.IsTrue(puttyKey.KeyLength == 1024);
            Assert.IsTrue(puttyKey.Algorithm == GnuPGAlgorithmType.RSA);
            Assert.IsTrue(puttyKey.CreationDate == new DateTime(2000, 12, 20));
            Assert.IsNull(puttyKey.KeyExpiration);
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
            string keyPath = TestUtils.KeyChainDirectory;
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
            GnuPGKey key = gpg.GetKeys().FirstOrDefault(s => s.KeyID == puttyPublicKeyID);
            if (key != null)
            {
                gpg.DeleteKey(key.KeyID);
                // Verify it isn't there
                key = gpg.GetKeys().FirstOrDefault(s => s.KeyID == puttyPublicKeyID);
                Assert.IsNull(key);
            }

            gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(puttyPublicKey)));
            key = gpg.GetKeys().FirstOrDefault(s => s.KeyID == puttyPublicKeyID);
            Assert.IsNotNull(key);

            string fingerprint = gpg.GetFingerprint(key.KeyID);
            Assert.IsTrue(fingerprint == puttyPublicKeyFingerprint);

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