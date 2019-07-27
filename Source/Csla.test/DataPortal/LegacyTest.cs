//-----------------------------------------------------------------------
// <copyright file="LegacyTest.cs" company="Marimer LLC">
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
    public class LegacyTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            Legacy test = Legacy.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            Legacy test = Legacy.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpInsert()
        {
            Legacy test = null;
            try
            {
                test = Legacy.NewObject();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpUpdate()
        {
            Legacy test = null;
            try
            {
                test = Legacy.NewObject();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            Legacy.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpDeleteSelf()
        {
            Legacy test = null;
            try
            {
                test = Legacy.NewObject();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Legacy"]);
        }

    }
}