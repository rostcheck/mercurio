using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mercurio.Domain.UnitTests
{
    [TestClass]
    public class AtomicDataElementTest
    {
        [TestMethod]
        public void AtomicDataElement_SameElement_returns_true_when_element_targets_are_same()
        {
            var change1 = AtomicDataElementChange.SetValue("target", 6, DataElementType.Integer);
            var change2 = AtomicDataElementChange.SetValue("target", 10, DataElementType.Integer);
            Assert.IsTrue(change1.SameElementAs(change2));
        }

        [TestMethod]
        public void AtomicDataElement_SameElement_returns_false_when_element_target_names_differ()
        {
            var change1 = AtomicDataElementChange.SetValue("target", 6, DataElementType.Integer);
            var change2 = AtomicDataElementChange.SetValue("target2", 10, DataElementType.Integer);
            Assert.IsFalse(change1.SameElementAs(change2));
        }

        [TestMethod]
        public void AtomicDataElement_SameElement_returns_false_when_element_target_types_differ()
        {
            var change1 = AtomicDataElementChange.SetValue("target", 6, DataElementType.Integer);
            var change2 = AtomicDataElementChange.SetValue("target", 10, DataElementType.String);
            Assert.IsFalse(change1.SameElementAs(change2));
        }
    }
}
