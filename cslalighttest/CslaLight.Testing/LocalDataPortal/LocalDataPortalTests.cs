using Csla.Core;
using Csla.Serialization.Mobile;
using cslalighttest.Engine;
using Csla.Testing.Business.EditableRootTests;

namespace cslalighttest.LocalDataPortal
{
  [TestClass]
  public class LocalDataPortalTests
  {
    [TestSetup]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Local"; // "Csla.DataPortalClient.WcfProxy, Csla";
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
