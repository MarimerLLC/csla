//-----------------------------------------------------------------------
// <copyright file="IOTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
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

namespace Csla.Test.IO
{
    [TestClass]
    public class IOTests
    {
        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void SaveNewRoot()
        {
            TestResults.Reinitialise();
            Csla.Test.Basic.Root root = NewRoot();

            root.Data = "saved";
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(true, root.IsDirty);
            Assert.AreEqual(true, root.IsValid);

            TestResults.Reinitialise();
            root = root.Save();

            Assert.IsNotNull(root);
            //fails because no call is being made to DataPortal_Insert in Root.DataPortal_Update if IsDeleted == false and IsNew == true
            Assert.AreEqual("Inserted", TestResults.GetResult("Root"));  
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(false, root.IsNew, "IsNew");
            Assert.AreEqual(false, root.IsDeleted, "IsDeleted");
            Assert.AreEqual(false, root.IsDirty, "IsDirty");
        }
        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void SaveOldRoot()
        {
            TestResults.Reinitialise();
            Csla.Test.Basic.Root root = GetRoot("old");

            root.Data = "saved";
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(true, root.IsDirty, "IsDirty");
            Assert.AreEqual(true, root.IsValid, "IsValid");

            TestResults.Reinitialise();
            root = root.Save();

            Assert.IsNotNull(root);
            Assert.AreEqual("Updated", TestResults.GetResult("Root"));
            Assert.AreEqual("saved", root.Data);
            Assert.AreEqual(false, root.IsNew, "IsNew");
            Assert.AreEqual(false, root.IsDeleted, "IsDeleted");
            Assert.AreEqual(false, root.IsDirty, "IsDirty");
        }
        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void LoadRoot()
        {
            TestResults.Reinitialise();
            Csla.Test.Basic.Root root = GetRoot("loaded");
            Assert.IsNotNull(root);
            Assert.AreEqual("Fetched", TestResults.GetResult("Root"));
            Assert.AreEqual("loaded", root.Data);
            Assert.AreEqual(false, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(false, root.IsDirty);
            Assert.AreEqual(true, root.IsValid);
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void DeleteNewRoot()
        {
            TestResults.Reinitialise();
            Csla.Test.Basic.Root root = NewRoot();

            TestResults.Reinitialise();
            root.Delete();
            Assert.AreEqual(true, root.IsNew);
            Assert.AreEqual(true, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);

            root = root.Save();
            Assert.IsNotNull(root);
            Assert.AreEqual(null, TestResults.GetResult("Root"));
            Assert.AreEqual(true, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void DeleteOldRoot()
        {
            TestResults.Reinitialise();
            Csla.Test.Basic.Root root = GetRoot("old");

            TestResults.Reinitialise();
            root.Delete();
            Assert.AreEqual(false, root.IsNew);
            Assert.AreEqual(true, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);

            root = root.Save();
            Assert.IsNotNull(root);
            Assert.AreEqual("Deleted self", TestResults.GetResult("Root"));
            Assert.AreEqual(true, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void DeleteRootImmediate()
        {
            TestResults.Reinitialise();
            DeleteRoot("test");
            Assert.AreEqual("Deleted", TestResults.GetResult("Root"));
        }

        [TestCleanup]
        public void ClearContextsAfterEachTest()
        {
            TestResults.Reinitialise();
        }

        private Basic.Root NewRoot()
        {
            IDataPortal<Basic.Root> dataPortal = DataPortalFactory.CreateDataPortal<Basic.Root>();

            return dataPortal.Create(new Basic.Root.Criteria());
        }

        private Basic.Root GetRoot(string data)
        {
            IDataPortal<Basic.Root> dataPortal = DataPortalFactory.CreateDataPortal<Basic.Root>();

            return dataPortal.Fetch(new Basic.Root.Criteria(data));
        }

        private void DeleteRoot(string data)
        {
          IDataPortal<Basic.Root> dataPortal = DataPortalFactory.CreateDataPortal<Basic.Root>();

          dataPortal.Delete(new Basic.Root.Criteria(data));
        }
    }
}