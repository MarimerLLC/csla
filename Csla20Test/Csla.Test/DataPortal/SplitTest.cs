using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using NUnit.Framework;

namespace Csla.Test.DataPortalTest
{
    [TestFixture]
    public class SplitTest
    {
        [Test]
        public void TestDpCreate()
        {
            Split test = Split.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["Split"]);
        }
        [Test]
        public void TestDpFetch()
        {
            Split test = Split.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Split"]);
        }
        [Test]
        public void TestDpInsert()
        {
            Split test = null;
            try
            {
                test = Split.NewObject();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Split"]);
        }
        [Test]
        public void TestDpUpdate()
        {
            Split test = null;
            try
            {
                test = Split.NewObject();
                test = test.Save();
                test.id = 5;
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Split"]);
        }
        [Test]
        public void TestDpDelete()
        {
            Split.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Split"]);
        }
        [Test]
        public void TestDpDeleteSelf()
        {
            Split test = null;
            try
            {
                test = Split.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Split"]);
        }

    }
}
