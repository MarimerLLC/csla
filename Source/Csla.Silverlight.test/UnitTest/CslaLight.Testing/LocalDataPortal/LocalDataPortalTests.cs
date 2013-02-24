//-----------------------------------------------------------------------
// <copyright file="LocalDataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Testing.Business.EditableRootTests;
using UnitDriven;

namespace cslalighttest.LocalDataPortal
{
  [TestClass]
  public class LocalDataPortalTests : TestBase
  {
    [TestInitialize]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Local"; // "Csla.DataPortalClient.WcfProxy, Csla";
    }

    [TestMethod]
    public void CanConstructTest()
    {
      UnitTestContext context = GetContext();
      MockEditableRoot root = new MockEditableRoot();
      context.Assert.Success();
    }

    [TestMethod]
    public void TestCreateNew()
    {
      UnitTestContext context = GetContext();
      MockEditableRoot.CreateNew((o, e) =>
      {
        MockEditableRoot actual = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(actual);
        context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
        context.Assert.IsTrue(actual.IsNew);
        context.Assert.IsTrue(actual.IsDirty);
        context.Assert.IsFalse(actual.IsDeleted);
        context.Assert.AreEqual("create", actual.DataPortalMethod);
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void TestInsert()
    {
      UnitTestContext context = GetContext();
      MockEditableRoot root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, true);
      root.Name = "justin";
      root.Saved += (o, e) =>
      {
        var actual = (MockEditableRoot)e.NewObject;
        context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
        context.Assert.IsFalse(actual.IsNew);
        context.Assert.IsFalse(actual.IsDirty);
        context.Assert.AreEqual("insert", actual.DataPortalMethod);
        context.Assert.Success();
      };
#if SILVERLIGHT
      root.BeginSave((o1, e1) => { context.Assert.IsNull(e1.Error); });
#else
      root.Save();
#endif 
      context.Complete();
    }

    [TestMethod]
    public void TestUpdate()
    {
      UnitTestContext context = GetContext();
      MockEditableRoot root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, false);
      root.Name = "justin";
      root.Saved += (o, e) =>
      {
        var actual = (MockEditableRoot)e.NewObject;
        context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
        context.Assert.IsFalse(actual.IsNew);
        context.Assert.IsFalse(actual.IsDirty);
        context.Assert.AreEqual("update", actual.DataPortalMethod);
        context.Assert.Success();
      };
#if SILVERLIGHT
      root.BeginSave((o1, e1) => { context.Assert.IsNull(e1.Error); });
#else
      root.Save();
#endif 
      context.Complete();
    }

    [TestMethod]
    public void TestDelete()
    {
      UnitTestContext context = GetContext();
      MockEditableRoot root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, false);
      root.Saved += (o, e) =>
      {
        var actual = (MockEditableRoot)e.NewObject;
        context.Assert.IsTrue(actual.IsNew);
        context.Assert.IsTrue(actual.IsDirty);
        context.Assert.IsFalse(actual.IsDeleted);
        context.Assert.AreEqual("delete", actual.DataPortalMethod);
        context.Assert.Success();
      };
      root.Delete();
#if SILVERLIGHT
      root.BeginSave((o1, e1) => { context.Assert.IsNull(e1.Error); });
#else
      root.Save();
#endif 
      context.Complete();
    }

    [TestMethod]
    public void TestFetch()
    {
      UnitTestContext context = GetContext();
      MockEditableRoot.Fetch(
      MockEditableRoot.MockEditableRootId,
      (o, e) =>
      {
        MockEditableRoot actual = e.Object;
        context.Assert.IsNull(e.Error);
        context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
        context.Assert.AreEqual("fetch", actual.DataPortalMethod);
        context.Assert.IsFalse(actual.IsNew);
        context.Assert.IsFalse(actual.IsDeleted);
        context.Assert.IsFalse(actual.IsDirty);
        context.Assert.Success();
      });
      context.Complete();
    }
  }
}