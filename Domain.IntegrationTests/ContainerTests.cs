using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domain.IntegrationTests
{
    [TestClass]
    public class ContainerTests
    {
        [TestMethod]
        public void Container_retains_all_revisions_when_retention_policy_is_keep_all()
        {
            var identity = Identity.Create("davidr-1", "public-key", "private-key");
            var user = User.Create("Test user", identity);

            var container = Container.Create("Container that keeps all");
            var document = container.CreateDocument("Test document");
            document.ChangeElement(AtomicDataElementChange.SetValue("name1", "value1", DataElementType.String));
            document.CommitChanges(identity.UniqueIdentifier);
            document.ChangeElement(AtomicDataElementChange.SetValue("name1", "value1", DataElementType.String));
            document.CommitChanges(identity.UniqueIdentifier);
            document.GetRevisions();
        }
    }
}
