using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using NUnit.Framework;

namespace Csla.Test.DataPortalTest
{
    [TestFixture]
    public class SingleTest
    {
        [Test]
        public void TestDpCreate()
        {
            Single test = Single.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["Single"]);
        }
        [Test]
        public void TestDpFetch()
        {
            Single test = Single.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Single"]);
        }
        [Test]
        public void TestDpInsert()
        {
            Single test = null;
            try
            {
                test = Single.NewObject();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Single"]);
        }
        [Test]
        public void TestDpUpdate()
        {
            Single test = null;
            try
            {
                test = Single.NewObject();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Single"]);
        }
        [Test]
        public void TestDpDelete()
        {
            Single.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Single"]);
        }
        [Test]
        public void TestDpDeleteSelf()
        {
            Single test = null;
            try
            {
                test = Single.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Single"]);
        }

    }
}
