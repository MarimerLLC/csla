//-----------------------------------------------------------------------
// <copyright file="SplitTest.cs" company="Marimer LLC">
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
    public class SplitTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            Split test = Split.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["Split"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            Split test = Split.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Split"]);
        }
        [TestMethod]
        public void TestDpInsert()
        {
            Split test = null;
            try
            {
                test = Split.NewObject();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Split"]);
        }
        [TestMethod]
        public void TestDpUpdate()
        {
            Split test = null;
            try
            {
                test = Split.NewObject();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Split"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            Split.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Split"]);
        }
        [TestMethod]
        public void TestDpDeleteSelf()
        {
            Split test = null;
            try
            {
                test = Split.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Split"]);
        }

    }
}