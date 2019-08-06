//-----------------------------------------------------------------------
// <copyright file="ChildParentDataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using System;
using Csla.Serialization;
using UnitDriven;
using cslalighttest.CslaDataProvider;

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


namespace cslalighttest.DataPortalTests
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class ChildParentDataPortalTests : TestBase
  {
    [TestInitialize]
    public void Setup()
    {
     Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
    }

    [TestMethod]
    public void ChildAccessesParentDataPortalTest()
    {
      var context = GetContext();

      Customer.GetCustomer(3, (o1, e1) =>
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        var cust = e1.Object;
        int custID = cust.Id;
        string custName = cust.Name;
        cust.Name = "new test name";
        cust.Contacts[0].FirstName = "Test F Name";
        cust.BeginSave((o, e) =>
          {
            Customer savedCust = (Customer)e.NewObject;
            context.Assert.IsNotNull(savedCust);
            if (savedCust != null)
            {
              context.Assert.AreEqual(savedCust.Method, "Updating Customer new test name");
              context.Assert.AreEqual(savedCust.Contacts[0].ParentName, "new test name");
              context.Assert.Success();
            }
          });
        
      });
      context.Complete();
    }
  }
}