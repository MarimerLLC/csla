using System;
using System.Threading;
using Csla.Testing.Business.EditableRootTests;
using cslalighttest.Engine;

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class EditableRootTests
  {
    [TestMethod]
    public void CanConstructTest()
    {
      MockEditableRoot root = new MockEditableRoot();
    }

    [TestMethod]
    public void TestCreateNew(AsyncTestContext context)
    {
      MockEditableRoot.CreateNew((actual, error) =>
      {
        context.Assert.IsNull(error);
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
        (actual, error) =>
        {
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
