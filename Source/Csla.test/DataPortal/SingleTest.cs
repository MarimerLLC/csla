//-----------------------------------------------------------------------
// <copyright file="SingleTest.cs" company="Marimer LLC">
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
    public class SingleTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            Single test = Single.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["Single"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            Single test = Single.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Single"]);
        }
        [TestMethod]
        public void TestDpInsert()
        {
            Single test = null;
            try
            {
                test = Single.NewObject();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Single"]);
        }
        [TestMethod]
        public void TestDpUpdate()
        {
            Single test = null;
            try
            {
                test = Single.NewObject();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Single"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            Single.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Single"]);
        }
        [TestMethod]
        public void TestDpDeleteSelf()
        {
            Single test = null;
            try
            {
                test = Single.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Single"]);
        }

    }
}