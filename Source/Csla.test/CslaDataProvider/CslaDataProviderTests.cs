//-----------------------------------------------------------------------
// <copyright file="CslaDataProviderTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Data;
using System.Data.SqlClient;
using Csla.Data;
using Csla.Test.Basic;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using System.Configuration;
using System.Linq;
using Csla.Test.Basic;
#endif

namespace Csla.Test.CslaDataProvider
{
  [TestClass]
  public class CslaDataProviderTests
  {
    [TestMethod]
    public void TestAddNew()
    {
      
      Csla.Test.Basic.RootList list = new Csla.Test.Basic.RootList();
      Csla.Xaml.CslaDataProvider dp = new Csla.Xaml.CslaDataProvider();
      dp.ObjectInstance = list;
      RootListChild child = dp.AddNew() as RootListChild;
      Assert.IsNotNull(child);
    }

    [TestMethod]
    public void TestAddNewReturnsNull()
    {
      Csla.Test.Basic.Root item = Csla.Test.Basic.Root.NewRoot();
      Csla.Xaml.CslaDataProvider dp = new Csla.Xaml.CslaDataProvider();
      dp.ObjectInstance = item;
      object child = dp.AddNew();
      Assert.IsNull(child);
    }

    [TestMethod]
    public void TestCancelError()
    {
      ProviderList list = ProviderList.GetList(); 
      Csla.Xaml.CslaDataProvider dp = new Csla.Xaml.CslaDataProvider();
      list.BeginEdit();
      dp.ManageObjectLifetime = true;
      dp.ObjectInstance = list;
      dp.RemoveItem(null, new Xaml.ExecuteEventArgs { MethodParameter = list[0] });
      dp.Save();

      Assert.AreEqual(1, list.Count);

      dp.Cancel();
      Assert.AreEqual(2, list.Count);

    }

    [TestMethod]
    public void TestSavedWithChanges()
    {
      Customer item = Customer.GetCustomer(1);
      Csla.Xaml.CslaDataProvider dp = new Csla.Xaml.CslaDataProvider();
      dp.ObjectInstance = item;
      item.Name = "New Name";
      bool saved = false;
      dp.Saved += (o, e) =>
        {
          Assert.IsNull(e.Error,"Error should be null");
          Assert.IsNotNull(e.NewObject, "Object should exist");
          Assert.AreEqual(((Customer)e.NewObject).Method, "Updating Customer New Name");
          saved = true;
        };
      dp.Save();
      Assert.IsTrue(saved);
    }

    [TestMethod]
    public void TestSavedWithChangesInvalid()
    {
      Customer item = Customer.GetCustomer(1);
      Csla.Xaml.CslaDataProvider dp = new Csla.Xaml.CslaDataProvider();
      dp.ObjectInstance = item;
      item.Name = "New Name";
      item.Id = 0;
      bool saved = false;
      dp.Saved += (o, e) =>
      {
        Assert.IsNotNull(e.Error, "Error should be null");
        saved = true;
      };
      dp.Save();
      Assert.IsTrue(saved);
    }

    [TestMethod]
    public void TestSavedWithoutChanges()
    {
      Customer item = Customer.GetCustomer(1);
      Csla.Xaml.CslaDataProvider dp = new Csla.Xaml.CslaDataProvider();
      dp.ObjectInstance = item;
      bool saved = false;
      dp.Saved += (o, e) =>
      {
        Assert.IsNull(e.Error, "Error should be null");
        Assert.IsNotNull(e.NewObject, "Object should exist");
        saved = true;
      };
      dp.Save();
      Assert.IsTrue(saved);
    }

    [TestCleanup]
    public void ClearContextsAfterEachTest() {
      Csla.ApplicationContext.GlobalContext.Clear();
    }
  }
}