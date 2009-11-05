using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using NUnit.Framework;

namespace Csla.Test.DataPortalTest
{
    [TestFixture]
    public class SingleOverloadTest
    {
        [Test]
        public void TestDpCreate()
        {
            SingleOverload test = SingleOverload.NewObject();
            Assert.AreEqual("Created0", ApplicationContext.GlobalContext["SingleOverload"]);
        }
        [Test]
        public void TestDpCreateWithCriteria()
        {
            SingleOverload test = SingleOverload.NewObjectWithCriteria();
            Assert.AreEqual("Created1", ApplicationContext.GlobalContext["SingleOverload"]);
        }
        [Test]
        public void TestDpFetch()
        {
            SingleOverload test = SingleOverload.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["SingleOverload"]);
        }
        [Test]
        public void TestDpDelete()
        {
            SingleOverload.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["SingleOverload"]);
        }

    }
}
