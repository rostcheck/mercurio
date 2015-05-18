using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mercurio.Domain;
using Mercurio.Domain.Implementation;
using MercurioShell;
using TestUtilities;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Utility;
using Mercurio.Domain.TestMocks;

namespace MercurioShell.UnitTests
{
    [TestClass]
    public class ShowContainerCommandTests
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
        public void ShowContainer_RecognizeCommand_recognizes_valid_command()
        {
            var command = new ShowContainerCommand();
            Assert.IsTrue(command.RecognizeCommand("show-containeRS"));
        }

        [TestMethod]
        public void ShowContainer_RecognizeCommand_recognizes_invalid_command()
        {
            var command = new ShowContainerCommand();
            Assert.IsFalse(command.RecognizeCommand("show-contttttttt"));
        }

        [TestMethod]
        public void ShowContainer_ValidateSyntax_validates_valid_syntax()
        {
            var command = new ShowContainerCommand();
            command.ValidateSyntax("Show-Containers", new Arguments(string.Empty.Split()));
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioShellSyntaxException))]
        public void ShowContainer_ValidateSyntax_validates_invalid_syntax()
        {
            var command = new ShowContainerCommand();
            command.ValidateSyntax("Show-Containers --whatever", new Arguments(string.Empty.Split()));
        }

        [TestMethod]
        public void ShowContainer_ShowHelp_shows_help()
        {
            var command = new ShowContainerCommand();
            Assert.IsTrue(command.ShowHelp() == "Usage: Show-Containers");
        }

        [TestMethod]
        public void ShowContainer_ExecuteCommand_finds_existing_containers()
        {
            var command = new ShowContainerCommand();
            var result = command.ExecuteCommand("Show-Containers", new Arguments(string.Empty.Split()), _context);
            Assert.IsTrue(result.Count == 0);
            _environment.SetActiveIdentity(_environment.GetAvailableIdentities().FirstOrDefault());
            _environment.CreateContainer("White",_environment.GetAvailableStorageSubstrateNames().First());
            result = command.ExecuteCommand("Show-Containers",  new Arguments(string.Empty.Split()), _context);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Contains("White"));
        }
    }
}
