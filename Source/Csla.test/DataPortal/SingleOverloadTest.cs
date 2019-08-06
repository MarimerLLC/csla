//-----------------------------------------------------------------------
// <copyright file="SingleOverloadTest.cs" company="Marimer LLC">
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
    public class SingleOverloadTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            SingleOverload test = SingleOverload.NewObject();
            Assert.AreEqual("Created0", ApplicationContext.GlobalContext["SingleOverload"]);
        }
        [TestMethod]
        public void TestDpCreateWithCriteria()
        {
            SingleOverload test = SingleOverload.NewObjectWithCriteria();
            Assert.AreEqual("Created1", ApplicationContext.GlobalContext["SingleOverload"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            SingleOverload test = SingleOverload.GetObject(5);
            Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["SingleOverload"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            SingleOverload.DeleteObject(5);
            Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["SingleOverload"]);
        }

    }
}