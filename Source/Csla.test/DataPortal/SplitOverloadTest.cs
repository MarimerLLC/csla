//-----------------------------------------------------------------------
// <copyright file="SplitOverloadTest.cs" company="Marimer LLC">
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
    public class SplitOverloadTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            SplitOverload test = SplitOverload.NewObject();
            Assert.AreEqual("Created", ApplicationContext.GlobalContext["SplitOverload"]);
        }
        [TestMethod]
        public void TestDpCreateWithCriteria()
        {
            SplitOverload test = SplitOverload.NewObjectWithCriteria();
            Assert.AreEqual("Created1", ApplicationContext.GlobalContext["SplitOverload"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            SplitOverload test = SplitOverload.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["SplitOverload"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            SplitOverload.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["SplitOverload"]);
        }

    }
}