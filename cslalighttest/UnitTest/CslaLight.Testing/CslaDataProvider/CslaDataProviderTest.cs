using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using Csla.Testing.Business.Security;
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
  [TestClass]
  public class CslaDataProviderTest : TestBase
  {
    [TestMethod]
    public void TestCslaDataProviderFetchNoParameters()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      //WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;

      Csla.Silverlight.CslaDataProvider provider = new Csla.Silverlight.CslaDataProvider();
      provider.PropertyChanged += (o1, e1) =>
        {
          if (e1.PropertyName == "Data")
          {
            context.Assert.AreEqual(true, ((Customer)provider.Data).Id > 0 && ((Customer)provider.Data).Id<11);
            context.Assert.Success();
          }
        };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.CreateFactoryMethod = "NewCustomer";
      provider.FetchFactoryMethod = "GetCustomer";
      provider.ObjectType = "cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";

      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderFetchWithParameter()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      //WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      int custId = (new Random()).Next(1, 10);
      Csla.Silverlight.CslaDataProvider provider = new Csla.Silverlight.CslaDataProvider();
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
      provider.CreateFactoryMethod = "NewCustomer";
      provider.FetchFactoryMethod = "GetCustomer";
      provider.ObjectType = "cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";

      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderCancel()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      //WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      Csla.Silverlight.CslaDataProvider provider = new Csla.Silverlight.CslaDataProvider();
      Customer.GetCustomer((o1,e1) =>
        {
          Customer cust = ((Customer)e1.Data);
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
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      //WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      Csla.Silverlight.CslaDataProvider provider = new Csla.Silverlight.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        Customer cust = ((Customer)e1.Data);
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
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      //WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
      Csla.Silverlight.CslaDataProvider provider = new Csla.Silverlight.CslaDataProvider();
      provider.ManageObjectLifetime = true;
      CustomerList.GetCustomerList((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        CustomerList custs = (CustomerList)e1.Data;
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
  }
}
