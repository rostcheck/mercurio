using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using MercurioShell;
using TestUtilities;
using System.Collections.Generic;
using System.Linq;

namespace MercurioShell.UnitTests
{
    [TestClass]
    public class ShowContainerCommandTests
    {
        private MercurioEnvironment _environment;
        private MercurioShellContext _context;
        private List<IStorageSubstrate> _substrates;
        public const string TestUserName = "mercurio";

        [TestInitialize]
        public void SetupTest()
        {
            TestUtils.CleanupSubstrate(ConfigurationManager.GetConfigurationValue("StorageSubstrate"));
            TestUtils.SetupUserDir(TestUserName);
            TestUtils.SwitchUser(null, TestUserName);

            var environmentScanner = new EnvironmentScanner(TestUtils.GetUserWorkingDir(TestUserName));
            _substrates = environmentScanner.GetStorageSubstrates();
            var serializer = SerializerFactory.Create(SerializerType.BinarySerializer);
            _environment = MercurioEnvironment.Create(environmentScanner, serializer, TestUtils.PassphraseFunction);
            _environment.SetUserHomeDirectory(TestUtils.GetUserWorkingDir(TestUserName));
            _context = new MercurioShellContext() { Environment = _environment };

            var identity = _environment.GetAvailableIdentities().Where(s => s.UniqueIdentifier == CryptoTestConstants.HermesPublicKeyID).FirstOrDefault();
            _environment.SetActiveIdentity(identity);
        }

        [TestMethod]
        public void ShowContainer_RecognizeCommand_recognizes_valid_command()
        {
            var command = new ShowContainerCommand(_context);
            Assert.IsTrue(command.RecognizeCommand("show-containeRS"));
        }

        [TestMethod]
        public void ShowContainer_RecognizeCommand_recognizes_invalid_command()
        {
            var command = new ShowContainerCommand(_context);
            Assert.IsFalse(command.RecognizeCommand("show-contttttttt"));
        }

        [TestMethod]
        public void ShowContainer_ValidateSyntax_validates_valid_syntax()
        {
            var command = new ShowContainerCommand(_context);
            Assert.IsTrue(command.ValidateSyntax("Show-Containers"));
        }

        [TestMethod]
        public void ShowContainer_ValidateSyntax_validates_invalid_syntax()
        {
            var command = new ShowContainerCommand(_context);
            Assert.IsFalse(command.ValidateSyntax("Show-Containers --whatever"));
        }

        [TestMethod]
        public void ShowContainer_ShowHelp_shows_help()
        {
            var command = new ShowContainerCommand(_context);
            Assert.IsTrue(command.ShowHelp() == "Usage: Show-Containers");
        }

        [TestMethod]
        public void ShowContainer_ExecuteCommand_finds_existing_containers()
        {
            var command = new ShowContainerCommand(_context);
            var result = command.ExecuteCommand("Show-Containers", _context);
            Assert.IsTrue(result.Count == 0);
            _environment.CreateContainer("White", _substrates[0].Name);
            result = command.ExecuteCommand("Show-Containers", _context);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Contains("White"));
        }
    }
}
