using CommandLine.Utility;
using Mercurio.Domain;
using Mercurio.Domain.TestMocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercurioShell.UnitTests
{
    [TestClass]
    public class CreateContainerCommandTests
    {
        private IMercurioEnvironment _environment;
        private MercurioShellContext _context;
        public const string TestUserName = "mercurio";

        [TestInitialize]
        public void SetupTest()
        {
            _environment = new MockMercurioEnvironment();
            _context = new MercurioShellContext() { Environment = _environment };
        }

        [TestMethod]
        public void CreateContainer_RecognizeCommand_recognizes_valid_command()
        {
            var command = new CreateContainerCommand();
            Assert.IsTrue(command.RecognizeCommand("create-containeR"));
        }

        [TestMethod]
        public void CreateContainer_RecognizeCommand_recognizes_invalid_command()
        {
            var command = new CreateContainerCommand();
            Assert.IsFalse(command.RecognizeCommand("create-containrr")); // missing required arguments
        }

        [TestMethod]
        public void CreateContainer_ValidateSyntax_validates_valid_syntax()
        {
            var command = new CreateContainerCommand();
            command.ValidateSyntax("Create-Container", new Arguments("-container-name Red -substrate-name MySubstrate".Split()));
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioShellSyntaxException))]
        public void CreateContainer_ValidateSyntax_validates_invalid_syntax()
        {
            var command = new CreateContainerCommand();
            command.ValidateSyntax("Create-Container", new Arguments(new string[] { "container-name", "Red" })); // Missing substrate name
        }

        [TestMethod]
        public void CreateContainer_ShowHelp_shows_help()
        {
            var command = new CreateContainerCommand();
            var result = command.ShowHelp();
            Assert.IsTrue(command.ShowHelp() == "Usage: Create-Container -container-name <name> -substrate-name <name> [-revision-retention-policy <KeepOne | KeepAll>]");
        }

       // [TestMethod]
        public void CreateContainer_ExecuteCommand_finds_existing_containers()
        {
            var command = new CreateContainerCommand();
            var result = command.ExecuteCommand("Create-Container", new Arguments(string.Empty.Split()), _context);
            Assert.IsTrue(result.Count == 0);
            _environment.SetActiveIdentity(_environment.GetAvailableIdentities().FirstOrDefault());
            _environment.CreateContainer("White", _environment.GetAvailableStorageSubstrateNames().First());
            result = command.ExecuteCommand("Create-Container", new Arguments(string.Empty.Split()), _context);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Contains("White"));
        }
    }
}
