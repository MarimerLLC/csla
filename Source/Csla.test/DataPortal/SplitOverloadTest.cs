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
    public class SplitOverloadTest
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
            IDataPortal<SplitOverload> dataPortal = _testDIContext.CreateDataPortal<SplitOverload>();

            SplitOverload test = SplitOverload.NewObject(dataPortal);
            Assert.AreEqual("Created", TestResults.GetResult("SplitOverload"));
        }
        [TestMethod]
        public void TestDpCreateWithCriteria()
        {
            IDataPortal<SplitOverload> dataPortal = _testDIContext.CreateDataPortal<SplitOverload>();

            SplitOverload test = SplitOverload.NewObjectWithCriteria(dataPortal);
            Assert.AreEqual("Created1", TestResults.GetResult("SplitOverload"));
        }
        [TestMethod]
        public void TestDpFetch()
        {
            IDataPortal<SplitOverload> dataPortal = _testDIContext.CreateDataPortal<SplitOverload>();
      
            SplitOverload test = SplitOverload.GetObject(5, dataPortal);
            Assert.AreEqual("Fetched", TestResults.GetResult("SplitOverload"));
        }
        [TestMethod]
        public void TestDpDelete()
        {
            IDataPortal<SplitOverload> dataPortal = _testDIContext.CreateDataPortal<SplitOverload>();
      
            SplitOverload.DeleteObject(5, dataPortal);
            Assert.AreEqual("Deleted", TestResults.GetResult("SplitOverload"));
        }

    }
}