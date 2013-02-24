//-----------------------------------------------------------------------
// <copyright file="CslaDataProviderTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Create is an exception - called with SingleCriteria, if BO does not have DP_Create() overload</summary>
//-----------------------------------------------------------------------
#if !WINDOWS_PHONE
using Csla;
using Csla.DataPortalClient;
using System;
using Csla.Serialization;
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
      DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
    }

    [TestMethod]
    public void When_Create_instantiates_Customer_with_random_id_between_1_and_10_DataSource_receives_that_record()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
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
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();
    }

    [TestMethod]
    public void When_Fetch_with_no_parameters_loads_Customer_with_random_id_between_1_and_10_DataSource_receives_that_record()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
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
      provider.FactoryMethod = "GetCustomer";
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();
    }

    [TestMethod]
    public void When_Fetch_called_with_random_value_between_1_and_10_parameter_DataSource_receives_that_record()
    {
      var context = GetContext();

      int custId = (new Random()).Next(1, 10);
      var provider = new Csla.Xaml.CslaDataProvider();
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
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();
    }

    [TestMethod]
    public void Cancel_reverts_property_values_on_bound_BO_back_to_the_original_values()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;
        provider.ObjectInstance = cust;
        cust.Name = "new test name";
        provider.Cancel();
        context.Assert.AreEqual(custID, ((Customer)provider.Data).Id);
        context.Assert.AreEqual(custName, ((Customer)provider.Data).Name);
        context.Assert.Success();
      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderSave()
    {
      using (var context = GetContext())
      {
        context.SetTimeout(TimeSpan.FromSeconds(30));
        var provider = new Csla.Xaml.CslaDataProvider();
        Customer.GetCustomer((o1, e1) =>
        {
          Csla.ApplicationContext.GlobalContext.Clear();
          var cust = e1.Object;
          int custID = cust.Id;
          string custName = cust.Name;
          provider.ObjectInstance = cust;
          cust.Name = "new test name";
          provider.PropertyChanged += (o2, e2) =>
          {
            if (e2.PropertyName == "Data")
            {
              context.Assert.AreEqual("Updating Customer new test name", ((Customer)provider.Data).Method);
              context.Assert.Success();
            }
            else if (e2.PropertyName == "Error")
            {
              context.Assert.Fail(provider.Error);
            }
          };
          provider.Save();
        });
        var tmp = provider.Data;
        context.Complete();
      }
    }

    [TestMethod]
    public void TestCslaDataProviderSavedEventTestWithChanges()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;
        provider.ObjectInstance = cust;
        cust.Name = "new test name";
        provider.Saved += (o2, e2) =>
        {
          context.Assert.AreEqual("Updating Customer new test name", ((Customer)provider.Data).Method);
          context.Assert.Success();

        };
        provider.Save();
      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderSavedEventTestWithoutChanges()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;
        provider.ObjectInstance = cust;
        provider.Saved += (o2, e2) =>
        {
          context.Assert.AreEqual(custName, ((Customer)provider.Data).Name);
          context.Assert.Success();

        };
        provider.Save();
      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderSavedEventTestWithInvalidException()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        cust.Id = 0;
        int custID = cust.Id;
        string custName = cust.Name;
        provider.ObjectInstance = cust;
        provider.Saved += (o2, e2) =>
        {
          context.Assert.IsNotNull(e2.Error);
          context.Assert.Success();

        };
        provider.Save();
      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void If_Fetch_returns_X_items_and_then_DataSource_removes_one_and_adds_two_Count_should_be_X_plus_1()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      provider.ManageObjectLifetime = true;
      CustomerList.GetCustomerList((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var custs = e1.Object;
        int count = custs.Count;
        provider.ObjectInstance = custs;
        provider.RemoveItem(null, new Csla.Xaml.ExecuteEventArgs { MethodParameter = custs[0] });
        provider.AddNewItem();
        provider.AddNewItem();
        context.Assert.AreEqual(count - 1 + 2, custs.Count);
        context.Assert.Success();

      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void IF_BO_Throws_Exception_DataSource_Error_property_contains_Exception_info()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
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
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();

    }

    [TestMethod]
    public void IF_BO_Throws_on_Fetch_Error_Is_Set()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      provider.DataChanged += (o1, e1) =>
        {
          context.Assert.IsNotNull(provider.Error);
          context.Assert.Success();
        };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "GetCustomerWithException";
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();

    }

    [TestMethod]
    public void Refresh_Calls_FactoryMethod_second_time()
    {
      var context = GetContext();
      int dataLoadedNTimes = 0;
      var provider = new Csla.Xaml.CslaDataProvider();
      provider.PropertyChanged += (o1, e1) =>
      {
        if (e1.PropertyName == "Data" && ++dataLoadedNTimes == 2)
        {
          context.Assert.Success();
        }
      };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "GetCustomer";
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.Customer, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      //Second call
      provider.Refresh();

      context.Complete();

    }
    [TestMethod]
    public void Fetch_call_on_BO_that_does_not_implement_DP_Fetch_returns_Exception_info_in_Error_property()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      provider.DataChanged += (o1, e1) =>
      {
        context.Assert.IsNotNull(provider.Error);
        context.Assert.Success();
      };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "GetCustomer";
      provider.ObjectType = typeof(CustomerWO_DP_XYZ).AssemblyQualifiedName;// "cslalighttest.CslaDataProvider.CustomerWO_DP_XYZ, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();

    }

    /// <summary>
    /// Create is an exception - called with SingleCriteria, if BO does not have DP_Create() overload
    /// with that signature, ends up calling parameterless DP_Create() - this is by design
    /// </summary>
    [TestMethod]
    public void Create_call_on_BO_that_does_not_implement_DP_Create_returns_no_Exception_info_in_Error_property()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      provider.DataChanged += (o1, e1) =>
      {
        context.Assert.IsNull(provider.Error);
        context.Assert.Success();
      };
      provider.IsInitialLoadEnabled = true;
      provider.ManageObjectLifetime = true;
      provider.FactoryMethod = "CreateCustomer";
      provider.ObjectType = typeof(CustomerWO_DP_XYZ).AssemblyQualifiedName;//"cslalighttest.CslaDataProvider.CustomerWO_DP_XYZ, Csla.Testing.Business, Version=..., Culture=neutral, PublicKeyToken=null";
      var tmp = provider.Data;

      context.Complete();

    }

    [TestMethod]
    public void TestCslaDataProviderCanOperationsObjectLevel()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;


        context.Assert.AreEqual(provider.CanEditObject, false);
        context.Assert.AreEqual(provider.CanGetObject, false);
        context.Assert.AreEqual(provider.CanDeleteObject, false);
        context.Assert.AreEqual(provider.CanCreateObject, false);

        System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();

        provider.PropertyChanged += (o3, e3) =>
          {
            list.Add(e3.PropertyName);
          };
        context.Completed += (o5, e5) =>
          {
            bool success = list.Contains("CanDeleteObject") &&
                              list.Contains("CanDeleteObject") &&
                              list.Contains("CanDeleteObject") &&
                              list.Contains("CanDeleteObject");
            context.Assert.IsTrue(success);
          };
        provider.ObjectInstance = cust;

        context.Assert.AreEqual(provider.CanEditObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, cust));
        context.Assert.AreEqual(provider.CanGetObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, cust));
        context.Assert.AreEqual(provider.CanDeleteObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, cust));
        context.Assert.AreEqual(provider.CanCreateObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, cust));

        context.Assert.AreEqual(provider.CanEditObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(Customer)));
        context.Assert.AreEqual(provider.CanGetObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(Customer)));
        context.Assert.AreEqual(provider.CanDeleteObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(Customer)));
        context.Assert.AreEqual(provider.CanCreateObject, Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(Customer)));
        context.Assert.Success();

      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderRebind()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;
        provider.ObjectInstance = cust;
        bool changedToNull = false;
        bool changedToData = false;

        provider.DataChanged += (o2, e2) =>
        {
          if (provider.Data == null)
            changedToNull = true;

          if (provider.Data == cust)
            changedToData = true;

          if (changedToNull && changedToData)
          {
            context.Assert.IsTrue(true);
            context.Assert.Success();
          }
        };
        provider.Rebind();

      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void TestCslaDataProviderCancelShouldBeBusy()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      Customer.GetCustomer((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        bool wasBusy = false;
        bool wasNotBusy = false;
        context.Completed += (o5, e5) =>
        {
          context.Assert.IsTrue(wasBusy);
          context.Assert.IsTrue(wasNotBusy);
        };
        provider.ObjectInstance = cust;
        cust.Name = "blah";
        provider.PropertyChanged += (o3, e3) =>
        {
          if (e3.PropertyName == "IsBusy" && provider.IsBusy)
            wasBusy = true;
          if (e3.PropertyName == "IsNotBusy" && provider.IsNotBusy && wasBusy)
            wasNotBusy = true;
        };
        provider.ObjectInstance = cust;
        provider.Cancel();
        context.Assert.Success();

      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void InheritedProviderFetch()
    {
      var context = GetContext();

      var provider = new InheritedProvider();
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
      provider.ObjectType = typeof(Customer).AssemblyQualifiedName;
      var tmp = provider.Data;

      context.Complete();
    }

    [TestMethod]
    public void InheritedProviderSave()
    {
      using (var context = GetContext())
      {
        context.SetTimeout(TimeSpan.FromSeconds(30));
        var provider = new InheritedProvider();
        Customer.GetCustomer((o1, e1) =>
        {
          Csla.ApplicationContext.GlobalContext.Clear();
          var cust = e1.Object;
          int custID = cust.Id;
          string custName = cust.Name;
          provider.ObjectInstance = cust;
          cust.Name = "new test name";
          provider.PropertyChanged += (o2, e2) =>
          {
            if (e2.PropertyName == "Data")
            {
              context.Assert.AreEqual("Updating Customer new test name", ((Customer)provider.Data).Method);
              context.Assert.Success();
            }
            else if (e2.PropertyName == "Error")
            {
              context.Assert.Fail(provider.Error);
            }
          };
          provider.Save();
        });
        var tmp = provider.Data;
      }
    }


    [TestMethod]
    public void ProviderCancelAfterExceptionInSave()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      provider.ManageObjectLifetime = true;
      CustomerWithErrorList.GetCustomerWithErrorList((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var custs = e1.Object;
        int count = custs.Count;
        provider.ObjectInstance = custs;
        provider.RemoveItem(null, new Csla.Xaml.ExecuteEventArgs { MethodParameter = custs[0] });


        provider.DataChanged += (o4, e4) =>
        {
          if (provider.Data != null)
          {
            CustomerWithErrorList savedList = provider.Data as CustomerWithErrorList;
            context.Assert.AreEqual(count, savedList.Count);
            context.Assert.Success();
          }
        };

        provider.PropertyChanged += (o3, e3) =>
          {
            if (e3.PropertyName == "Error")
            {
              provider.Cancel();
            }

          };
        provider.Save();


      });
      var tmp = provider.Data;
      context.Complete();
    }

    [TestMethod]
    public void ProviderCancelAfterSuccessfulSave()
    {
      var context = GetContext();

      var provider = new Csla.Xaml.CslaDataProvider();
      provider.ManageObjectLifetime = true;
      CustomerList.GetCustomerList((o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var custs = e1.Object;
        int count = custs.Count;
        provider.ManageObjectLifetime = true;
        provider.ObjectInstance = custs;
        provider.RemoveItem(null, new Csla.Xaml.ExecuteEventArgs { MethodParameter = custs[0] });
        bool continueTest = true;
        provider.DataChanged += (o3, e3) =>
        {
          if (continueTest)
          {
            continueTest = false;
            CustomerList savedList = provider.Data as CustomerList;
            context.Assert.AreEqual(count - 1, savedList.Count);
            context.Assert.Success();
          }

        };
        provider.Save();


      });
      var tmp = provider.Data;
      context.Complete();
    }
  }

}
#endif
