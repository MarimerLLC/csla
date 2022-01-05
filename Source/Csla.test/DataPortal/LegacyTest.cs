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
    public class LegacyTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            Legacy test = NewLegacy();
            Assert.AreEqual("Created", TestResults.GetResult("Legacy"));
        }
        [TestMethod]
        public void TestDpFetch()
        {
            Legacy test = GetLegacy(5);
            Assert.AreEqual("Fetched", TestResults.GetResult("Legacy"));
        }
        [TestMethod]
        public void TestDpInsert()
        {
            Legacy test = null;
            try
            {
                test = NewLegacy();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Inserted", TestResults.GetResult("Legacy"));
        }
        [TestMethod]
        public void TestDpUpdate()
        {
            Legacy test = null;
            try
            {
                test = NewLegacy();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", TestResults.GetResult("Legacy"));
        }
        [TestMethod]
        public void TestDpDelete()
        {
            DeleteLegacy(5);
            Assert.AreEqual("Deleted", TestResults.GetResult("Legacy"));
        }
        [TestMethod]
        public void TestDpDeleteSelf()
        {
            Legacy test = null;
            try
            {
                test = NewLegacy();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", TestResults.GetResult("Legacy"));
        }
        
        private Legacy NewLegacy()
        {
            IDataPortal<Legacy> dataPortal = DataPortalFactory.CreateDataPortal<Legacy>();

            return dataPortal.Create();
        }

        private Legacy GetLegacy(int id)
        {
            IDataPortal<Legacy> dataPortal = DataPortalFactory.CreateDataPortal<Legacy>();

            return dataPortal.Fetch(new Legacy.Criteria(id));
        }

        private void DeleteLegacy(int id)
        {
            IDataPortal<Legacy> dataPortal = DataPortalFactory.CreateDataPortal<Legacy>();

            dataPortal.Delete(new Legacy.Criteria(id));
        }
    }
}