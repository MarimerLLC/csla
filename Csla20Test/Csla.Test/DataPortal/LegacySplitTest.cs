using System;
using System.Collections.Generic;
using System.Text;
using Csla;

using NUnit.Framework;

namespace Csla.Test.DataPortalTest
{
    [TestFixture]
    public class LegacySplitTest
    {
        [Test]
        public void TestDpCreate()
        {
            LegacySplit test = LegacySplit.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [Test]
        public void TestDpFetch()
        {
            LegacySplit test = LegacySplit.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [Test]
        public void TestDpInsert()
        {
            LegacySplit test = LegacySplit.NewObject();
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [Test]
        public void TestDpUpdate()
        {
            LegacySplit test = null;
            try
            {
                test = LegacySplit.NewObject();
                test = test.Save();
                test.id = 5;
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [Test]
        public void TestDpDelete()
        {
            LegacySplit.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [Test]
        public void TestDpDeleteSelf()
        {
            LegacySplit test = null;
            try
            {
                test = LegacySplit.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Ignore(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["LegacySplit"]);
        }

    }
}
