using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Starksoft.Cryptography.OpenPGP;
using TestUtils;

namespace TestCryptography
{
    [TestClass]
    public class GnuPGTest
    {
        [TestMethod]
        public void ListKeysTest()
        {
            GnuPG gpg = PrepareTest();
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

        private GnuPG PrepareTest()
        {
            string userName = "mercurio";
            TestUtils.TestUtils.SetupDirs(new List<string>() { userName });
            string keyPath = TestUtils.TestUtils.GetUserDir(userName);
            string binaryPath = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) + "\\GNU\\GnuPG\\gpg2.exe";
            return new GnuPG(keyPath, binaryPath);
        }

        [TestMethod]
        [ExpectedException(typeof(GnuPGException))]
        public void ImportBadKeyTest()
        {
            GnuPG gpg = PrepareTest(); 
            string badKey = "This is not a valid key";
            gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(badKey)));
        }

        [TestMethod]
        public void ImportGoodKeyTest()
        {
            GnuPG gpg = PrepareTest();

            // If the key is on the ring, delete it
            GnuPGKey key = gpg.GetKeys().FirstOrDefault(s => s.Key == "B41CAE29");
            if (key != null) 
            {
                gpg.DeleteKey(key.Key);
                // Verify it isn't there
                key = gpg.GetKeys().FirstOrDefault(s => s.Key == "B41CAE29");
                Assert.IsNull(key);
            }
            string goodKey = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
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

            gpg.Import(new MemoryStream(Encoding.ASCII.GetBytes(goodKey)));
            key = gpg.GetKeys().FirstOrDefault(s => s.Key == "B41CAE29");
            Assert.IsNotNull(key);
        }
    }
}