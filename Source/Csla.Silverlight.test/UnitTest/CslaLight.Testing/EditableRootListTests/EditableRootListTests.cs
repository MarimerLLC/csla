//-----------------------------------------------------------------------
// <copyright file="EditableRootListTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using System.ComponentModel;
#if !__ANDROID__
using System.Windows.Controls;
#endif
using Csla.Testing.Business.Security;
using UnitDriven;
using Csla.Testing.Business.EditableRootListTests;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

#if !__ANDROID__
namespace cslalighttest.EditableRootListTests
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class EditableRootListTests : TestBase
  {
    [TestMethod]
    public void AddItem()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
        {
          if (e.Error != null)
            context.Assert.Fail(e.Error.Message);
          else
          {
            context.Assert.IsNotNull(e.Object);
            list = e.Object;
            SingleItem item = SingleItem.GetSingleItem();
            list.Add(item);
            context.Assert.AreEqual(3, list.Count, "Count should be 3");
            context.Assert.IsTrue(list[2].IsNew, "Object should be new");
          }
          context.Assert.Success();
        }
        );
      context.Complete();
    }

    [TestMethod]
    public void RemoveNewItem()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.IsNotNull(e.Object);
        list = e.Object;
        SingleItem item = SingleItem.GetSingleItem();
        list.Add(item);
        context.Assert.AreEqual(3, list.Count, "Count should be 3");
        context.Assert.IsTrue(list[2].IsNew, "Object should be new");

        list.RemoveAt(2);
        context.Assert.AreEqual(2, list.Count, "Incorrect count after remove");
        context.Assert.AreEqual(false, item.MethodCalled == "DataPortal_DeleteSelf", "Should not have deleted new item");
        context.Assert.IsTrue(item.IsNew, "Object should be new after delete");

        context.Assert.Success();
      }
        );
      context.Complete();


    }

    [TestMethod]
    public void RemoveNewItemViaListSaved()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.IsNotNull(e.Object);
        list = e.Object;
        SingleItem item = SingleItem.GetSingleItem();
        list.Add(item);
        context.Assert.AreEqual(3, list.Count, "Count should be 3");
        context.Assert.IsTrue(list[2].IsNew, "Object should be new");
        list.Saved += (o1, e1) =>
        {
          context.Assert.AreEqual(2, list.Count, "Incorrect count after remove");
          context.Assert.AreEqual(false, ((SingleItem)e1.NewObject).MethodCalled == "DataPortal_DeleteSelf", "Should not have deleted new item");
          context.Assert.IsTrue(item.IsNew, "Object should be new after delete");

          context.Assert.Success();
        };
        list.RemoveAt(2);

      }
      );
      context.Complete();


    }

    [TestMethod]
    public void RemoveOldItem()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.IsNotNull(e.Object);
        list = e.Object;
        SingleItem item = SingleItem.GetSingleItem();
        list.Add(item);
        context.Assert.AreEqual(3, list.Count, "Count should be 3");
        context.Assert.IsTrue(list[2].IsNew, "Object should be new");
        list[0].Saved += (o1, e1) =>
          {
            context.Assert.IsNull(e1.Error);
            context.Assert.AreEqual(2, list.Count, "Incorrect count after remove");
            context.Assert.AreEqual("DataPortal_DeleteSelf",
                  ((SingleItem)e1.NewObject).MethodCalled);
            context.Assert.Success();
          };
        list.RemoveAt(0);

      }
        );
      context.Complete();
    }

    [TestMethod]
    public void InsertItem()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.IsNotNull(e.Object);
        list = e.Object;
        SingleItem item = SingleItem.GetSingleItem();
        list.Add(item);
        context.Assert.AreEqual(3, list.Count, "Count should be 3");
        context.Assert.IsTrue(list[2].IsNew, "Object should be new");
        list[2].Saved += (o1, e1) =>
        {
          context.Assert.IsNull(e1.Error);
          context.Assert.AreEqual(3, list.Count, "Incorrect count after remove");
          context.Assert.AreEqual("DataPortal_Insert", ((SingleItem)e1.NewObject).MethodCalled, "Object should have been inserted");
          context.Assert.IsFalse(list[2].IsNew, "Object should not be new");
          context.Assert.Success();
        };

        // simulate grid edit
        item = list[2];
        IEditableObject obj = (IEditableObject)item;
        obj.BeginEdit();
        item.Name = "test";
        obj.EndEdit();
      }
        );
      context.Complete();

    }

    [TestMethod]
    public void UpdateItem()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.IsNotNull(e.Object);
        list = e.Object;
        context.Assert.AreEqual(2, list.Count, "Count should be 2");
        list[0].Saved += (o1, e1) =>
        {
          context.Assert.IsNull(e1.Error);
          context.Assert.AreEqual(2, list.Count, "Incorrect count after remove");
          context.Assert.AreEqual("DataPortal_Update", ((SingleItem)e1.NewObject).MethodCalled, "Object should have been updated");
          context.Assert.IsFalse(list[0].IsDirty, "Object should not be dirty");
          context.Assert.Success();
        };

        // simulate grid edit
        SingleItem item = list[0];
        IEditableObject obj = (IEditableObject)item;
        obj.BeginEdit();
        item.Name = "test";
        obj.EndEdit();
      }
        );
      context.Complete();

    }

    [TestMethod]
    public void UpdateItemViaListSavedEvent()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.Try(() =>
        {
          context.Assert.IsNotNull(e.Object);
          RootSingleItemsList list = e.Object;
          context.Assert.AreEqual(2, list.Count, "Count should be 2");
          list.Saved += (o1, e1) =>
          {
            context.Assert.IsNull(e1.Error);
            context.Assert.AreEqual(2, list.Count, "Incorrect count after remove");
            context.Assert.AreEqual("DataPortal_Update", ((SingleItem)e1.NewObject).MethodCalled, "Object should have been updated");
            context.Assert.IsFalse(list[0].IsDirty, "Object should not be dirty");
            context.Assert.Success();
          };

        // simulate grid edit
        SingleItem item = list[0];
        IEditableObject obj = (IEditableObject)item;
        obj.BeginEdit();
        item.Name = "test";
        obj.EndEdit();
        });
      });
      context.Complete();
    }

    [TestMethod]
    public void BusyImplemented()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      ApplicationContext.GlobalContext.Clear();

      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        context.Assert.IsFalse(e.Object.IsBusy);
        context.Assert.Success();
      });
      context.Complete();
    }
  }
}
#endif
