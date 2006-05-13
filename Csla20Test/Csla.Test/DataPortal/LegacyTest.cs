using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using NUnit.Framework;

namespace Csla.Test.DataPortalTest
{
    [TestFixture]
    public class LegacyTest
    {
        [Test]
        public void TestDpCreate()
        {
            Legacy test = Legacy.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["Legacy"]);
        }
        [Test]
        public void TestDpFetch()
        {
            Legacy test = Legacy.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Legacy"]);
        }
        [Test]
        public void TestDpInsert()
        {
            Legacy test = null;
            try
            {
                test = Legacy.NewObject();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Legacy"]);
        }
        [Test]
        public void TestDpUpdate()
        {
            Legacy test = null;
            try
            {
                test = Legacy.NewObject();
                test = test.Save();
                test.id = 5;
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Legacy"]);
        }
        [Test]
        public void TestDpDelete()
        {
            Legacy.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Legacy"]);
        }
        [Test]
        public void TestDpDeleteSelf()
        {
            Legacy test = null;
            try
            {
                test = Legacy.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Legacy"]);
        }

    }
}
