using System;
using MercurioShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MercurioShell.IntegrationTests
{
    [TestClass]
    public class MercurioShellTests
    {
        [TestMethod]
        public void MercurioShell_constructs_and_executes_commands()
        {
            var commands = "Show-Containers";
            var shell = new MercurioShell.MercurioCommandShell(commands);
            var result = shell.ExecuteCommand(commands);
            Assert.IsNotNull(result);
           // Assert.IsTrue(result)
        }
    }
}
