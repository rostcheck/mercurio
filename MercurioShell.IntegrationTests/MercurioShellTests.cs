using System;
using MercurioShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using TestUtilities;
using Mercurio.Domain.Implementation;
using System.Linq;

namespace MercurioShell.IntegrationTests
{
    [TestClass]
    public class MercurioShellTests
    {
        public const string TestUserName = "mercurio";
        private IMercurioEnvironment _environment;

        [TestInitialize]
        public void TestInitialize()
        {
            TestUtils.CleanupSubstrate(ConfigurationManager.GetConfigurationValue("StorageSubstrate"));
            TestUtils.SetupUserDir(TestUserName);
            TestUtils.SwitchUser(null, TestUserName);
            SetupEnvironment();
        }

        private void SetupEnvironment()
        {
            var environmentScanner = new EnvironmentScanner(TestUtils.GetUserWorkingDir(TestUserName));
            var storageSubstrates = environmentScanner.GetStorageSubstrates();
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            var osAbstractor = OSAbstractorFactory.GetOsAbstractor();
            _environment = MercurioEnvironment.Create(environmentScanner, osAbstractor, serializer, TestUtils.PassphraseFunction);
            _environment.SetUserHomeDirectory(TestUtils.GetUserWorkingDir(TestUserName));
            var identity = _environment.GetAvailableIdentities().Where(s => s.UniqueIdentifier == CryptoTestConstants.HermesPublicKeyID).FirstOrDefault();
            _environment.SetActiveIdentity(identity);
        }

        [TestMethod]
        public void MercurioShell_constructs_and_executes_commands()
        {
            var commands = "Show-Containers";
            var shell = MercurioCommandShell.Create(_environment, ConfirmAction);
            var result = shell.ExecuteCommand(commands);
            Assert.IsTrue(result.Count == 0);

            commands = "Create-Container";
            result = shell.ExecuteCommand("Create-Container -container-name White -substrate-name MercurioStorage");

            result = shell.ExecuteCommand("Show-Containers");
            Assert.IsFalse(result.Count == 0);
        }

        private static bool ConfirmAction(string prompt, IMercurioEnvironment environment)
        {
            return true;
        }
    }
}
