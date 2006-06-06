using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using NUnit.Framework;

namespace Csla.Test.DataPortalTest
{
    [TestFixture]
    public class SplitOverloadTest
    {
        [Test]
        public void TestDpCreate()
        {
            SplitOverload test = SplitOverload.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["SplitOverload"]);
        }
        [Test]
        public void TestDpCreateWithCriteria()
        {
            SplitOverload test = SplitOverload.NewObjectWithCriteria();
            Assert.AreEqual("Created1", ApplicationContext.GlobalContext["SplitOverload"]);
        }
        [Test]
        public void TestDpFetch()
        {
            SplitOverload test = SplitOverload.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["SplitOverload"]);
        }
        [Test]
        public void TestDpDelete()
        {
            SplitOverload.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["SplitOverload"]);
        }

    }
}
