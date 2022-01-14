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
using Csla.TestHelpers;

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
        private static TestDIContext _testDIContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _testDIContext = TestDIContextFactory.CreateDefaultContext();
        }

        [TestMethod]
        public void TestDpCreate()
        {
            IDataPortal<SingleOverload> dataPortal = _testDIContext.CreateDataPortal<SingleOverload>();

            SingleOverload test = SingleOverload.NewObject(dataPortal);
            Assert.AreEqual("Created0", TestResults.GetResult("SingleOverload"));
        }
        [TestMethod]
        public void TestDpCreateWithCriteria()
        {
            IDataPortal<SingleOverload> dataPortal = _testDIContext.CreateDataPortal<SingleOverload>();

            SingleOverload test = SingleOverload.NewObjectWithCriteria(dataPortal);
            Assert.AreEqual("Created1", TestResults.GetResult("SingleOverload"));
        }
        [TestMethod]
        public void TestDpFetch()
        {
            IDataPortal<SingleOverload> dataPortal = _testDIContext.CreateDataPortal<SingleOverload>();

            SingleOverload test = SingleOverload.GetObject(5, dataPortal);
            Assert.AreEqual("Fetched", TestResults.GetResult("SingleOverload"));
        }
        [TestMethod]
        public void TestDpDelete()
        {
            IDataPortal<SingleOverload> dataPortal = _testDIContext.CreateDataPortal<SingleOverload>();

            SingleOverload.DeleteObject(5, dataPortal);
            Assert.AreEqual("Deleted", TestResults.GetResult("SingleOverload"));
        }

    }
}