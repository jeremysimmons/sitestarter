using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void Test_Example()
        {
            Assert.AreEqual(1, 1, "This should not fail.");
        }

        [Test]
        public void Test_FailedExample()
        {
            Assert.AreEqual(1, 2, "This will fail intentionally.");
        }
    }
}
