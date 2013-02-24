//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using Csla.Testing.Business.Security;
using UnitDriven;
using Csla.Testing.Business.ObjectFactory;

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


namespace cslalighttest.ObjectFactory
{
  [TestClass]
  public class ObjectFactoryLocalProxyTests : TestBase
  {
    [TestMethod]
    public void TestNoFactoryFetchNoParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.GetBusinessItem((o, e) =>
       {
         context.Assert.IsNull(e.Error);
         context.Assert.IsNotNull(e.Object);
         context.Assert.AreEqual(e.Object.Id, "random_fetch");
         context.Assert.AreEqual(e.Object.OperationResult, "DataPortal_Fetch/no parameters");
         context.Assert.Success();
       });
      context.Complete();
    }

    [TestMethod]
    public void TestNoFactoryFetchParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.GetBusinessItem("an id",(o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "fetch_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "DataPortal_Fetch/with parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestNoFactoryCreateNoParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.NewBusinessItem((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "random_create");
        context.Assert.AreEqual(e.Object.OperationResult, "DataPortal_Create/no parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestNoFactoryCreateParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.NewBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "create_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "DataPortal_Create/with parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestNoFactoryDelete()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.DeleteBusinessItem("an id to delete", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "delete_an id to delete");
        context.Assert.AreEqual(e.Object.OperationResult, "DeleteObjectFactoryBusinessItem/with parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestNoFactoryInsert()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.NewBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "create_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "DataPortal_Create/with parameters");
        e.Object.OperationResult = "about to insert";
        e.Object.BeginSave((o1, e1) =>
          {
            context.Assert.IsNull(e1.Error);
            context.Assert.IsNotNull(e1.NewObject);
            context.Assert.AreEqual(((BusinessItem)e1.NewObject).Id, "random_insert");
            context.Assert.AreEqual(((BusinessItem)e1.NewObject).OperationResult, "DataPortal_Insert");
            context.Assert.Success();
          });
      });
      context.Complete();
    }

    [TestMethod]
    public void TestNoFactoryUpdate()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      BusinessItem.GetBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "fetch_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "DataPortal_Fetch/with parameters");
        e.Object.OperationResult = "about to update";
        e.Object.BeginSave((o1, e1) =>
        {
          context.Assert.IsNull(e1.Error);
          context.Assert.IsNotNull(e1.NewObject);
          context.Assert.AreEqual(((BusinessItem)e1.NewObject).Id, "random_update");
          context.Assert.AreEqual(((BusinessItem)e1.NewObject).OperationResult, "DataPortal_Update");
          context.Assert.Success();
        });
      });
      context.Complete();
    }



    [TestMethod]
    public void TestWithFactoryFetchNoParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.GetObjectFactoryBusinessItem((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(false, e.Object.IsDirty);
        context.Assert.AreEqual(e.Object.Id, "random_fetch");
        context.Assert.AreEqual(e.Object.OperationResult, "FetchObjectFactoryBusinessItem/no parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestWithFactoryFetchParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.GetObjectFactoryBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(false, e.Object.IsDirty);
        context.Assert.AreEqual(e.Object.Id, "fetch_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "FetchObjectFactoryBusinessItem/with parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestWithFactoryCreateNoParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.NewObjectFactoryBusinessItem((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(true, e.Object.IsNew);
        context.Assert.AreEqual(e.Object.Id, "random_create");
        context.Assert.AreEqual(e.Object.OperationResult, "CreateObjectFactoryBusinessItem/no parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestWithFactoryCreateParams()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.NewObjectFactoryBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(true, e.Object.IsNew);
        context.Assert.AreEqual(e.Object.Id, "create_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "CreateObjectFactoryBusinessItem/with parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestWithFactoryDelete()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.DeleteObjectFactoryBusinessItem("an id to delete", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "delete_an id to delete");
        context.Assert.AreEqual(e.Object.OperationResult, "DeleteObjectFactoryBusinessItem/with parameters");
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod]
    public void TestWithFactoryInsert()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.NewObjectFactoryBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "create_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "CreateObjectFactoryBusinessItem/with parameters");
        e.Object.OperationResult = "about to insert";
        e.Object.BeginSave((o1, e1) =>
        {
          context.Assert.IsNull(e1.Error);
          context.Assert.IsNotNull(e1.NewObject);
          context.Assert.AreEqual(((ObjectFactoryBusinessItem)e1.NewObject).Id, "inserted");
          context.Assert.AreEqual(((ObjectFactoryBusinessItem)e1.NewObject).OperationResult, "UpdateObjectFactoryBusinessItem/with parameters");
          context.Assert.Success();
        });
      });
      context.Complete();
    }

    [TestMethod]
    public void TestWithFactoryUpdate()
    {
      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Local";
      ObjectFactoryBusinessItem.GetObjectFactoryBusinessItem("an id", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(e.Object.Id, "fetch_an id");
        context.Assert.AreEqual(e.Object.OperationResult, "FetchObjectFactoryBusinessItem/with parameters");
        e.Object.OperationResult = "about to update";
        e.Object.BeginSave((o1, e1) =>
        {
          context.Assert.IsNull(e1.Error);
          context.Assert.IsNotNull(e1.NewObject);
          context.Assert.AreEqual(((ObjectFactoryBusinessItem)e1.NewObject).Id, "updated");
          context.Assert.AreEqual(((ObjectFactoryBusinessItem)e1.NewObject).OperationResult, "UpdateObjectFactoryBusinessItem/with parameters");
          context.Assert.Success();
        });
      });
      context.Complete();
    }

  }
}