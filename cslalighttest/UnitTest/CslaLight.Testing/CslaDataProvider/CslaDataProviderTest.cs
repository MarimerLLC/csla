using Csla;
using Csla.DataPortalClient;
//using Csla.Testing.Business.ReadOnlyTest;
using System;
//using Csla.Testing.Business.Security;
using UnitDriven;

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

namespace cslalighttest.CslaDataProvider
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class CslaDataProviderTest : TestBase
  {
    [TestInitialize]
    public void Setup()
    {
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy<>).AssemblyQualifiedName;
      //WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
    }

    [TestMethod]
    public void TestCslaDataProviderCreate()
    {
      var context = GetContext();

      var provider = new Csla.Silverlight.CslaDataProvider();
      provider.PropertyChanged += (o1, e1) =>
      {
        if (e1.PropertyName == "Data")
        {
          var customer = (Customer)provider.Data;
          context.Assert.AreEqual(true, customer.Id > 0 && customer.Id < 11);
          context.Assert.Success();
        }
      };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "CreateCustomer";
      provider.ObjectType = "cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";

      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderFetchNoParameters()
    {
      var context = GetContext();

      var provider = new Csla.Silverlight.CslaDataProvider();
      provider.PropertyChanged += (o1, e1) =>
        {
          if (e1.PropertyName == "Data")
          {
            var customer = (Customer)provider.Data;
            context.Assert.AreEqual(true, customer.Id > 0 && customer.Id<11);
            context.Assert.Success();
          }
        };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "GetCustomer";
      provider.ObjectType = "cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";

      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderFetchWithParameter()
    {
      var context = GetContext();

      int custId = (new Random()).Next(1, 10);
      var provider = new Csla.Silverlight.CslaDataProvider();
      provider.PropertyChanged += (o1, e1) =>
      {
        if (e1.PropertyName == "Data")
        {
          context.Assert.AreEqual(custId, ((Customer)provider.Data).Id);
          context.Assert.Success();
        }
      };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryParameters.Add(custId);
      provider.FactoryMethod = "GetCustomer";
      provider.ObjectType = "cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";

      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderCancel()
    {
      var context = GetContext();

      var provider = new Csla.Silverlight.CslaDataProvider();
      Customer.GetCustomer((o1,e1) =>
        {
          var cust = e1.Object;
          int custID = cust.Id;
          string custName = cust.Name;
          provider.Data = cust;
          cust.Name = "new test name";
          provider.Cancel();
          context.Assert.AreEqual(custID, ((Customer)provider.Data).Id);
          context.Assert.AreEqual(custName, ((Customer)provider.Data).Name);
          context.Assert.Success();
        });
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderSave()
    {
      var context = GetContext();

      var provider = new Csla.Silverlight.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;
        provider.Data = cust;
        cust.Name = "new test name";
        provider.PropertyChanged += (o2, e2) =>
        {
          if (e2.PropertyName == "Data")
          {
            context.Assert.AreEqual("Updating Customer new test name", ((Customer)provider.Data).Method);
            context.Assert.Success();
          }
        };
        provider.Save();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderAddRemove()
    {
      var context = GetContext();

      var provider = new Csla.Silverlight.CslaDataProvider();
      provider.ManageObjectLifetime = true;
      CustomerList.GetCustomerList((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var custs = e1.Object;
        int count = custs.Count;
        provider.Data = custs;
        provider.RemoveItem(custs[0]);
        provider.AddNewItem();
        provider.AddNewItem();
        context.Assert.AreEqual(count -1 + 2,custs.Count);
        context.Assert.Success();

      });
      context.Complete();
    }

    [TestMethod]
    public void IF_BO_Throws_Exception_DataSource_Error_property_contains_Exception_info()
    {
      var context = GetContext();

      var provider = new Csla.Silverlight.CslaDataProvider();
      provider.PropertyChanged += (o1, e1) =>
      {
        if (e1.PropertyName == "Error")
        {
          context.Assert.IsNotNull(provider.Error);
          context.Assert.Success();
        }
      };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "GetCustomerWithException";
      provider.ObjectType = "cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";

      context.Complete();
      
    }
  }
}
