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
    public class LegacySplitTest
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
            LegacySplit test = NewLegacySplit();
            Assert.AreEqual("Created", TestResults.GetResult("LegacySplit"));
        }

        [TestMethod]
        public void TestDpFetch()
        {
            LegacySplit test = GetLegacySplit(5);
            Assert.AreEqual("Fetched", TestResults.GetResult("LegacySplit"));
        }

        [TestMethod]
        public void TestDpInsert()
        {
            LegacySplit test = NewLegacySplit();
            test.Save();
            Assert.AreEqual("Inserted", TestResults.GetResult("LegacySplit"));
        }

        [TestMethod]
        public void TestDpUpdate()
        {
            LegacySplit test = null;
            try
            {
                test = NewLegacySplit();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", TestResults.GetResult("LegacySplit"));
        }

        [TestMethod]
        public void TestDpDelete()
        {
            DeleteLegacySplit(5);
            Assert.AreEqual("Deleted", TestResults.GetResult("LegacySplit"));
        }

        [TestMethod]
        public void TestDpDeleteSelf()
        {
            LegacySplit test = null;
            try
            {
                test = NewLegacySplit();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", TestResults.GetResult("LegacySplit"));
        }

        private LegacySplit NewLegacySplit()
        {
            IDataPortal<LegacySplit> dataPortal = _testDIContext.CreateDataPortal<LegacySplit>();

            return dataPortal.Create();
        }

        private LegacySplit GetLegacySplit(int id)
        {
            IDataPortal<LegacySplit> dataPortal = _testDIContext.CreateDataPortal<LegacySplit>();

            return dataPortal.Fetch(new LegacySplitBase<LegacySplit>.Criteria(id));
        }

        private void DeleteLegacySplit(int id)
        {
            IDataPortal<LegacySplit> dataPortal = _testDIContext.CreateDataPortal<LegacySplit>();

            dataPortal.Delete(new LegacySplitBase<LegacySplit>.Criteria(id));
        }
    }
}