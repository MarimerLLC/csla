using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using Csla.Testing.Business.Security;
using UnitDriven;
using Csla.Testing.Business.BusyStatus;

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

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusyStatusTests : TestBase
  {

    [TestMethod]
    public void TestBusy()
    {
#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
        {
          item = e.Object;
          context.Assert.IsNull(e.Error);
          context.Assert.IsNotNull(item);


          item.RuleField = "some value";
          context.Assert.IsTrue(item.IsBusy);
          context.Assert.IsFalse(item.IsSavable);
          context.Assert.Success();
        });
      context.Complete();
    }

    [TestMethod]
    public void ListTestBusy()
    {
#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TestSaveWhileBusy()
    {
#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#endif      
      UnitTestContext context = GetContext();
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
      {
        

        item = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(item);


        item.RuleField = "some value";
        context.Assert.IsTrue(item.IsBusy);
        context.Assert.IsFalse(item.IsSavable);

        item.BeginSave((o1, e1) =>
          {
            Csla.Rules.ValidationException error = e1.Error as Csla.Rules.ValidationException;
            context.Assert.IsNotNull(error);
            if (error != null)
              context.Assert.IsTrue(error.Message.ToLower().Contains("busy"));
            context.Assert.IsTrue(error.Message.ToLower().Contains("save"));
            context.Assert.Success();
          });
        
      });
      context.Complete();
    }

    [TestMethod]
    public void ListTestSaveWhileBusy()
    {
#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);

      items.BeginSave((o1, e1) =>
      {
        var error = e1.Error as Csla.Rules.ValidationException;
        context.Assert.IsNotNull(error);
        if (error != null)
          context.Assert.IsTrue(error.Message.ToLower().Contains("busy"));
        context.Assert.IsTrue(error.Message.ToLower().Contains("save"));
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void TestNotBusy()
    {
#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
      {


        item = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(item);


        item.RuleField = "some value";
        context.Assert.IsTrue(item.IsBusy);
        context.Assert.IsFalse(item.IsSavable);
        item.ValidationComplete += (o2, e2) =>
          {
            context.Assert.IsFalse(item.IsBusy);
            context.Assert.IsTrue(item.IsSavable);
            context.Assert.Success();
          };
      });
      context.Complete();
    }

    [TestMethod]
    public void ListTestNotBusy()
    {

#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].ValidationComplete += (o2, e2) =>
      {
        context.Assert.IsFalse(items.IsBusy);
        context.Assert.IsTrue(items.IsSavable);
        context.Assert.Success();
      };

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
    }

    [TestMethod]
    public void TestSaveWhileNotBusy()
    {
#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#endif
     UnitTestContext context = GetContext();
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
      {

        item = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(item);


        item.RuleField = "some value";
        context.Assert.IsTrue(item.IsBusy);
        context.Assert.IsFalse(item.IsSavable);
        item.ValidationComplete += (o2, e2) =>
        {
          context.Assert.IsFalse(item.IsBusy);
          context.Assert.IsTrue(item.IsSavable);
          item.BeginSave((o4, e4) =>
            {
              context.Assert.IsNull(e4.Error);
              context.Assert.IsNotNull(e4.NewObject);
              item = (ItemWithAsynchRule)e4.NewObject;
              if (item != null)
                context.Assert.AreEqual("DataPortal_Update", item.OperationResult);
              context.Assert.Success();
            });
        };
      });
      context.Complete();
    }


    [TestMethod]
    public void ListTestSaveWhileNotBusy()
    {

#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].ValidationComplete += (o2, e2) =>
      {
        context.Assert.IsFalse(items.IsBusy);
        context.Assert.IsTrue(items.IsSavable);
        items.BeginSave((o4, e4) =>
        {
          context.Assert.IsNull(e4.Error);
          context.Assert.IsNotNull(e4.NewObject);
          items = (ItemWithAsynchRuleList)e4.NewObject;
          context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
          context.Assert.Success();
        });
      };

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      context.Complete();
    }

#if !SILVERLIGHT
    [TestMethod]
    public void TestSaveWhileBusyNetOnly()
    {

      UnitTestContext context = GetContext();
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
      {


        item = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(item);


        item.RuleField = "some value";
        context.Assert.IsTrue(item.IsBusy);
        context.Assert.IsFalse(item.IsSavable);
        bool gotError = false;
        try
        {
          item.Save();
        }
        catch (Csla.Rules.ValidationException EX)
        {
          gotError = true;
        }
        context.Assert.IsTrue(gotError);
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void ListTestSaveWhileBusyNetOnly()
    {


#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      bool gotError = false;
      try
      {
        items.Save();
      }
      catch (Csla.Rules.ValidationException EX)
      {
        gotError = true;
      }
      context.Assert.IsTrue(gotError);
      context.Assert.Success();

      context.Complete();


    }

    [TestMethod]
    public void TestSaveWhileNotBusyNetOnly()
    {
      UnitTestContext context = GetContext();
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
      {

        item = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(item);


        item.RuleField = "some value";
        context.Assert.IsTrue(item.IsBusy);
        context.Assert.IsFalse(item.IsSavable);
        item.ValidationComplete += (o2, e2) =>
        {
          context.Assert.IsFalse(item.IsBusy);
          context.Assert.IsTrue(item.IsSavable);
          item = item.Save();
          context.Assert.AreEqual("DataPortal_Update", item.OperationResult);
          context.Assert.Success();
        };
      });
      context.Complete();
    }

    [TestMethod]
    public void ListTestSaveWhileNotBusyNetOnly()
    {


#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();

     

      items[0].ValidationComplete += (o2, e2) =>
      {
        context.Assert.IsFalse(items.IsBusy);
        context.Assert.IsTrue(items.IsSavable);
        items = items.Save();
        context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
        context.Assert.Success();
      };

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);

      context.Complete();

    }

    [TestMethod]
    public void TestSaveWhileNotBusyNoActiveRuleNetOnly()
    {
      UnitTestContext context = GetContext();
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      ItemWithAsynchRule item;
      ItemWithAsynchRule.GetItemWithAsynchRule("an id", (o, e) =>
      {

        item = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(item);


        item.OperationResult = "something";
        context.Assert.IsFalse(item.IsBusy);
        context.Assert.IsTrue(item.IsSavable);
        item = item.Save();
        context.Assert.AreEqual("DataPortal_Update", item.OperationResult);
        context.Assert.Success();
      });
      context.Complete();
    }


    [TestMethod]
    public void ListTestSaveWhileNotBusyNoActiveRuleNetOnly()
    {



#if SILVERLIGHT
          DataPortal.ProxyTypeName = "Local";
#else
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
#endif
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();

      items[0].OperationResult = "something";
      context.Assert.IsFalse(items.IsBusy);
      context.Assert.IsTrue(items.IsSavable);
      items = items.Save();
      context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
      context.Assert.Success();
      context.Complete();

    }
#endif
  }
}
