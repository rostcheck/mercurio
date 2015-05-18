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
    public class HelpCommandTests
    {
        private IMercurioEnvironment _environment;
        private MercurioShellContext _context;
        public const string TestUserName = "mercurio";

        [TestInitialize]
        public void SetupTest()
        {
            _environment = new MockMercurioEnvironment();
            _context = new MercurioShellContext() { Environment = _environment, Commands = new List<IExecutableMercurioCommand>() };
        }

        [TestMethod]
        public void Help_RecognizeCommand_recognizes_valid_command()
        {
            var command = new HelpCommand();
            Assert.IsTrue(command.RecognizeCommand("heLP"));
        }

        [TestMethod]
        public void Help_RecognizeCommand_recognizes_invalid_command()
        {
            var command = new HelpCommand();
            Assert.IsFalse(command.RecognizeCommand("helllll"));
        }

        [TestMethod]
        public void Help_ValidateSyntax_validates_valid_syntax()
        {
            var command = new HelpCommand();
            command.ValidateSyntax("Help", new Arguments(string.Empty.Split()));
        }

        [TestMethod]
        [ExpectedException(typeof(MercurioShellSyntaxException))]
        public void Help_ValidateSyntax_validates_invalid_syntax()
        {
            var command = new HelpCommand();
            command.ValidateSyntax("Help --whatever", new Arguments(string.Empty.Split()));
        }

        [TestMethod]
        public void Help_ShowHelp_shows_help()
        {
            var command = new HelpCommand();
            var result = command.ShowHelp();
            Assert.IsTrue(command.ShowHelp() == "Usage: Help [-Subject <name>]");
        }

        [TestMethod]
        public void Help_ExecuteCommand_calls_GetHelp_on_argument()
        {
            var command = new HelpCommand();
            var targetCommand = new ShowContainerCommand();
            _context.Commands.Add(targetCommand);
            // Verify calling help with explicit -subject
            var result = command.ExecuteCommand("Help", new Arguments(new string[] {"-subject", "Show-Containers"}), _context);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.First() == targetCommand.ShowHelp());
        }

        [TestMethod]
        public void Help_RewriteBeforeParsing_adds_subject_argument()
        {
            var command = new HelpCommand();
            var rewritten = command.RewriteBeforeParsing("help show-containers");
            var args = new Arguments(rewritten.Split());
            Assert.IsNotNull(args["subject"]);
        }
    }
}
