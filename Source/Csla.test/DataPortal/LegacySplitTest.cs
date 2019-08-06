//-----------------------------------------------------------------------
// <copyright file="LegacySplitTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.DataPortalTest
{
    [TestClass]
    public class LegacySplitTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            LegacySplit test = LegacySplit.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            LegacySplit test = LegacySplit.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [TestMethod]
        public void TestDpInsert()
        {
            LegacySplit test = LegacySplit.NewObject();
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [TestMethod]
        public void TestDpUpdate()
        {
            LegacySplit test = null;
            try
            {
                test = LegacySplit.NewObject();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            LegacySplit.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["LegacySplit"]);
        }
        [TestMethod]
        public void TestDpDeleteSelf()
        {
            LegacySplit test = null;
            try
            {
                test = LegacySplit.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["LegacySplit"]);
        }

    }
}