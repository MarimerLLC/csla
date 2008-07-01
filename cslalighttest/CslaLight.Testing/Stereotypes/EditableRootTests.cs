using System;
using System.Threading;
using Csla.Testing.Business.EditableRootTests;
using cslalighttest.Engine;
using System.Diagnostics;
using cslalighttest.Properties;
using Csla;

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class EditableRootTests
  {
    [TestSetup]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = Resources.RemotePortalUrl;
    }

    [TestMethod]
    public void CanConstructTest()
    {
      MockEditableRoot root = new MockEditableRoot();
    }

    [TestMethod]
    public void TestCreateNew(AsyncTestContext context)
    {
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
    }

    [TestMethod]
    public void TestInsert(AsyncTestContext context)
    {
      MockEditableRoot root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, true);
      root.Name = "justin";
      root.Saved += (o, e) =>
      {
        var actual = (MockEditableRoot)e.NewObject;
        context.Assert.IsNotNull(actual);
        context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
        context.Assert.IsFalse(actual.IsNew);
        context.Assert.IsFalse(actual.IsDirty);
        context.Assert.AreEqual("insert", actual.DataPortalMethod);
        context.Assert.Success();
      };
      root.Save();
    }

    [TestMethod]
    public void TestUpdate(AsyncTestContext context)
    {
      MockEditableRoot root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, false);
      root.Name = "justin";
      root.Saved += (o, e) =>
      {
        var actual = (MockEditableRoot)e.NewObject;
        context.Assert.IsNotNull(actual);
        context.Assert.AreEqual(MockEditableRoot.MockEditableRootId, actual.Id);
        context.Assert.IsFalse(actual.IsNew);
        context.Assert.IsFalse(actual.IsDirty);
        context.Assert.AreEqual("update", actual.DataPortalMethod);
        context.Assert.Success();
      };
      root.Save();
    }

    [TestMethod]
    public void TestDelete(AsyncTestContext context)
    {
      MockEditableRoot root = new MockEditableRoot(MockEditableRoot.MockEditableRootId, false);
      root.Saved += (o, e) =>
      {
        var actual = (MockEditableRoot)e.NewObject;
        context.Assert.IsNotNull(actual);
        context.Assert.IsTrue(actual.IsNew);
        context.Assert.IsTrue(actual.IsDirty);
        context.Assert.IsFalse(actual.IsDeleted);
        context.Assert.AreEqual("delete", actual.DataPortalMethod);
        context.Assert.Success();
      };
      root.Delete();
      root.Save();
    }

    [TestMethod]
    public void TestFetch(AsyncTestContext context)
    {
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
    }
  }
}
