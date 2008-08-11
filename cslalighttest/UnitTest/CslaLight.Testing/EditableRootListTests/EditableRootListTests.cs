using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
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

namespace cslalighttest.EditableRootListTests
{
  [TestClass]
  public class EditableRootListTests : TestBase
  {
    [TestMethod]
    public void AddItem()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
        {
          ApplicationContext.GlobalContext.Clear();
          context.Assert.IsNotNull(e.Object);
          list = e.Object;
          SingleItem item = SingleItem.GetSingleItem();
          list.Add(item);
          Assert.AreEqual(3, list.Count, "Count should be 3");
          Assert.IsTrue(list[2].IsNew, "Object should be new");
          context.Assert.Success();
        }
        );
      context.Complete();
    }

    [TestMethod]
    public void RemoveNewItem()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;

      RootSingleItemsList list;
      RootSingleItemsList.GetRootSingleItemsList(1, 2, (o, e) =>
      {
        ApplicationContext.GlobalContext.Clear();
        context.Assert.IsNotNull(e.Object);
        list = e.Object;
        SingleItem item = SingleItem.GetSingleItem();
        list.Add(item);
        Assert.AreEqual(3, list.Count, "Count should be 3");
        Assert.IsTrue(list[2].IsNew, "Object should be new");

        list.RemoveAt(2);
        context.Assert.AreEqual(2, list.Count, "Incorrect count after remove");
        context.Assert.IsNull(ApplicationContext.GlobalContext["ERLBDeleteSelf"], "Object should not have done a delete");
        Assert.IsTrue(item.IsNew, "Object should be new after delete");

        context.Assert.Success();
      }
        );
      context.Complete();

     
    }

  }
}
