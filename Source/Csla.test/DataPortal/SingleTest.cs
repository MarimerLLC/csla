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
    public class SingleTest
    {
        [TestMethod]
        public void TestDpCreate()
        {
            Single test = NewSingle();
            Assert.AreEqual("Created", TestResults.GetResult("Single"));
        }
        [TestMethod]
        public void TestDpFetch()
        {
            Single test = GetSingle(5);
            Assert.AreEqual("Fetched", TestResults.GetResult("Single"));
        }
        [TestMethod]
        public void TestDpInsert()
        {
            Single test = null;
            try
            {
                test = NewSingle();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Inserted", TestResults.GetResult("Single"));
        }
        [TestMethod]
        public void TestDpUpdate()
        {
            Single test = null;
            try
            {
                test = NewSingle();
                test = test.Save();
                test.Id = 5;
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("Updated", TestResults.GetResult("Single"));
        }
        [TestMethod]
        public void TestDpDelete()
        {
            DeleteSingle(5);
            Assert.AreEqual("Deleted", TestResults.GetResult("Single"));
        }
        [TestMethod]
        public void TestDpDeleteSelf()
        {
            Single test = null;
            try
            {
                test = NewSingle();
                test = test.Save();
                test.Delete();
            }
            catch { Assert.Inconclusive(); }
            test.Save();
            Assert.AreEqual("SelfDeleted", TestResults.GetResult("Single"));
        }

        private Single NewSingle()
        {
            IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

            return dataPortal.Create();
        }

        private Single GetSingle(int id)
        {
            IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

            return dataPortal.Fetch(id);
        }

        private void DeleteSingle(int id)
        {
            IDataPortal<Single> dataPortal = DataPortalFactory.CreateDataPortal<Single>();

            dataPortal.Delete(id);
        }
  }
}