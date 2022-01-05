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
            // TODO: Fix test
            //Assert.AreEqual("Created", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpFetch()
        {
            Legacy test = GetLegacy(5);
            // TODO: Fix test
            //Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Legacy"]);
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
            // TODO: Fix test
            //Assert.AreEqual("Inserted", ApplicationContext.GlobalContext["Legacy"]);
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
            // TODO: Fix test
            //Assert.AreEqual("Updated", ApplicationContext.GlobalContext["Legacy"]);
        }
        [TestMethod]
        public void TestDpDelete()
        {
            DeleteLegacy(5);
            // TODO: Fix test
            // Assert.AreEqual("Deleted", ApplicationContext.GlobalContext["Legacy"]);
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
            // TODO: Fix test
            // Assert.AreEqual("SelfDeleted", ApplicationContext.GlobalContext["Legacy"]);
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