using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestUtilities;

namespace Mercurio.Domain.TestMocks
{
    public class MockCryptoManager : ICryptoManager
    {
        private NetworkCredential _credential = null;
        Dictionary<byte[], byte[]> _cleartexts;

        public MockCryptoManager()
        {
            _cleartexts = new Dictionary<byte[], byte[]>();
        }

        public void SetCredential(NetworkCredential credential)
        {
            _credential = credential;
        }

        public void SetConfiguration(CryptoManagerConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredential(NetworkCredential credential)
        {
            if (_credential == null)
                return false;

            return true;
        }

        public string GetActiveIdentity()
        {
            return (_credential == null) ? string.Empty : _credential.UserName;
        }

        public System.IO.Stream Encrypt(System.IO.Stream messageStream, string identifier)
        {
            var random = new Random();
            using (var memoryStream = new MemoryStream())
            {
                messageStream.Position = 0;
                messageStream.CopyTo(memoryStream);
                var clearTextBytes = memoryStream.ToArray();
                var fakeEncryptedBytes = new byte[(int)(clearTextBytes.Length * 1.3)];
                random.NextBytes(fakeEncryptedBytes);
                _cleartexts.Add(fakeEncryptedBytes, clearTextBytes);
                return new MemoryStream(fakeEncryptedBytes);
            }
        }

        public string Encrypt(string message, string identifier)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream Decrypt(System.IO.Stream messageStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                messageStream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                var fakeEncryptedBytes = memoryStream.ToArray();
                if (_cleartexts.ContainsKey(fakeEncryptedBytes))
                {
                    var fakeDecryptedStream = new MemoryStream(_cleartexts[fakeEncryptedBytes]);
                    fakeDecryptedStream.Position = 0;
                    return fakeDecryptedStream;
                }
                else
                    throw new Exception("Cannot decrypt");
            }
        }

        public string Sign(string message)
        {
            throw new NotImplementedException();
        }

        public bool Validate(string message)
        {
            throw new NotImplementedException();
        }

        public string GetPublicKey(string identifier)
        {
            throw new NotImplementedException();
        }

        public string ImportKey(string key)
        {
            throw new NotImplementedException();
        }

        public void SignKey(string identifier)
        {
            throw new NotImplementedException();
        }

        public string[] GetSignatures()
        {
            throw new NotImplementedException();
        }

        public string GetFingerprint(string identifier)
        {
            return "99288213";
        }

        public void DeleteKey(string identifier)
        {
            throw new NotImplementedException();
        }

        public bool HasPublicKey(string key)
        {
            throw new NotImplementedException();
        }

        public List<UserIdentity> GetAvailableIdentities()
        {
            var result = new List<UserIdentity>();
            result.Add(UserIdentity.Create(CryptoTestConstants.HermesPublicKeyID, "Test Identity", null, "Test User Identity", "MockCrypto"));
            return result;
        }

        public List<ContactIdentity> GetAvailableContactIdentities()
        {
            throw new NotImplementedException();
        }

        public void CreateKey(string identifier, NetworkCredential credential)
        {
            throw new NotImplementedException();
        }

        public string ManagerType
        {
            get { return "MockCryptoManager"; }
        }
    }
}
