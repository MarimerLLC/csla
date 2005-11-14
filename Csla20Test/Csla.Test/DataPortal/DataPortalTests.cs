using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.DataBinding;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.DataPortal
{
    [TestClass()]
    public class DataPortalTests
    {
        [TestMethod()]
        public void DataPortalEvents()
        {
            Csla.DataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
            Csla.DataPortal.DataPortalInvokeComplete += new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);

            try
            {
                ApplicationContext.GlobalContext.Clear();
                DpRoot root = DpRoot.NewRoot();

                root.Data = "saved";
                Csla.ApplicationContext.GlobalContext.Clear();
                root = root.Save();

                Assert.IsTrue((bool)ApplicationContext.GlobalContext["dpinvoke"], "DataPortalInvoke not called");
                Assert.IsTrue((bool)ApplicationContext.GlobalContext["dpinvokecomplete"], "DataPortalInvokeComplete not called");
                Assert.IsTrue((bool)ApplicationContext.GlobalContext["serverinvoke"], "Server DataPortalInvoke not called");
                Assert.IsTrue((bool)ApplicationContext.GlobalContext["serverinvokecomplete"], "Server DataPortalInvokeComplete not called");
            }
            finally
            {
                Csla.DataPortal.DataPortalInvoke -= new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvoke);
                Csla.DataPortal.DataPortalInvokeComplete -= new Action<DataPortalEventArgs>(ClientPortal_DataPortalInvokeComplete);
            }
        }

        [Test]
        public void CallDataPortalOverrides()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            ParentEntity parent = ParentEntity.NewParentEntity();
            parent.Data = "something";

            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(true, parent.IsNew);
            Assert.AreEqual(true, parent.IsDirty);
            Assert.AreEqual(true, parent.IsSavable);

            parent = parent.Save();

            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["ParentEntity"]);

            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(false, parent.IsNew);
            Assert.AreEqual(false, parent.IsDirty);
            Assert.AreEqual(false, parent.IsSavable);

            parent.Data = "something new";

            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(false, parent.IsNew);
            Assert.AreEqual(true, parent.IsDirty);
            Assert.AreEqual(true, parent.IsSavable);

            parent = parent.Save();

            Assert.AreEqual("Updated", Csla.ApplicationContext.GlobalContext["ParentEntity"]);

            parent.Delete();
            Assert.AreEqual(true, parent.IsDeleted);
            parent = parent.Save();
            Assert.AreEqual("Deleted Self", Csla.ApplicationContext.GlobalContext["ParentEntity"]);

            ParentEntity.DeleteParentEntity(33);
            Assert.AreEqual("Deleted", Csla.ApplicationContext.GlobalContext["ParentEntity"]);
            Assert.AreEqual(false, parent.IsDeleted);
            Assert.AreEqual(true, parent.IsValid);
            Assert.AreEqual(true, parent.IsNew);
            Assert.AreEqual(true, parent.IsDirty);
            Assert.AreEqual(true, parent.IsSavable);

            ParentEntity.GetParentEntity(33);
            Assert.AreEqual("Fetched", Csla.ApplicationContext.GlobalContext["ParentEntity"]);
        }

        private void ClientPortal_DataPortalInvoke(DataPortalEventArgs obj)
        {
            ApplicationContext.GlobalContext["dpinvoke"] = true;
        }

        private void ClientPortal_DataPortalInvokeComplete(DataPortalEventArgs obj)
        {
            ApplicationContext.GlobalContext["dpinvokecomplete"] = true;
        }
        
    }
}
