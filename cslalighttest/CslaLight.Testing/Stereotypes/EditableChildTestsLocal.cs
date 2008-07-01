
using cslalighttest.Engine;
using Csla;
using Csla.Testing.Business.EditableChildTests;
using System;
namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class EditableChildTestsLocal
  {
    [TestSetup]
    public void Setup()
    {
      DataPortal.ProxyTypeName = "Local";
    }

    [TestMethod]
    public void ListWithChildrenNoCriteria()
    {
      MockList.FetchAll((o, e) =>
      {
        MockList l = (MockList)e.Object;
        Assert.IsNull(e.Error);
        Assert.IsNotNull(l);
        Assert.AreEqual(3, l.Count);
        Assert.AreEqual(MockList.MockEditableChildId1, l[0].Id);
        Assert.AreEqual("Child_Fetch", l[0].DataPortalMethod);
        Assert.AreEqual(1, l[0].GrandChildren.Count);
        Assert.AreEqual(GrandChildList.GrandChildId1, l[0].GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", l[0].GrandChildren[0].DataPortalMethod);

        Assert.AreEqual(MockList.MockEditableChildId2, l[1].Id);
        Assert.AreEqual("Child_Fetch", l[1].DataPortalMethod);
        Assert.AreEqual(1, l[1].GrandChildren.Count);
        Assert.AreEqual(GrandChildList.GrandChildId2, l[1].GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", l[1].GrandChildren[0].DataPortalMethod);

        Assert.AreEqual(MockList.MockEditableChildId3, l[2].Id);
        Assert.AreEqual("Child_Fetch", l[2].DataPortalMethod);
        Assert.AreEqual(1, l[2].GrandChildren.Count);
        Assert.AreEqual(GrandChildList.GrandChildId3, l[2].GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", l[2].GrandChildren[0].DataPortalMethod);
      });
    }

    [TestMethod]
    public void ListWithChildrenCriteria()
    {
      MockList.FetchByName("c2", (o, e) =>
      {
        MockList l = (MockList)e.Object;
        Assert.IsNull(e.Error);
        Assert.IsNotNull(l);
        Assert.AreEqual(1, l.Count);
        Assert.AreEqual(MockList.MockEditableChildId2, l[0].Id);
        Assert.AreEqual("Child_Fetch", l[0].DataPortalMethod);
        Assert.IsFalse(l[0].IsNew);
        Assert.IsFalse(l[0].IsDirty);

        Assert.AreEqual(1, l[0].GrandChildren.Count);
        Assert.AreEqual(GrandChildList.GrandChildId2, l[0].GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", l[0].GrandChildren[0].DataPortalMethod);
        Assert.IsFalse(l[0].GrandChildren[0].IsNew);
        Assert.IsFalse(l[0].GrandChildren[0].IsDirty);
      });
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void SaveChildFail(AsyncTestContext context)
    {
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.Try(() => e.Object[0].Save());
      });
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void SaveGrandChildListFail(AsyncTestContext context)
    {
      MockList.FetchByName("c2", 
      (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.Try(() => e.Object[0].GrandChildren.Save());
        context.Assert.Fail();
      });
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void SaveGrandChildFail(AsyncTestContext context)
    {
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.Try(() => e.Object[0].GrandChildren[0].Save());
        context.Assert.Fail();
      });
    }

    [TestMethod]
    public void UpdateMockEditableChild(AsyncTestContext context)
    {
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(1, e.Object.Count);
        context.Assert.AreEqual("c2", e.Object[0].Name);

        e.Object[0].Name = "saving";
        e.Object.Saved += (o2, e2) =>
        {
          MockList l2 = (MockList)e2.NewObject;
          context.Assert.IsNotNull(l2);

          context.Assert.AreEqual(e.Object.Count, l2.Count);
          context.Assert.AreEqual(e.Object[0].Id, l2[0].Id);
          context.Assert.AreEqual(e.Object[0].Name, l2[0].Name);

          context.Assert.AreEqual("Child_Fetch", e.Object[0].DataPortalMethod);
          context.Assert.AreEqual("Child_Update", l2[0].DataPortalMethod);

          context.Assert.IsTrue(e.Object[0].IsDirty);
          context.Assert.IsFalse(l2[0].IsDirty);

          context.Assert.AreEqual(e.Object[0].GrandChildren.Count, l2[0].GrandChildren.Count);

          l2[0].GrandChildren[0].Name = "saving";
          l2.Saved += (o3, e3) =>
          {
            MockList l3 = (MockList)e3.NewObject;
            context.Assert.AreEqual("Child_Fetch", l2[0].GrandChildren[0].DataPortalMethod);
            context.Assert.IsTrue(l2[0].GrandChildren[0].IsDirty);
            
            context.Assert.AreEqual("Child_Update", l3[0].GrandChildren[0].DataPortalMethod);
            context.Assert.IsFalse(l3[0].GrandChildren[0].IsDirty);

            context.Assert.Success();
          };
          l2.Save();
        };
        e.Object.Save();
      });
    }
  }
}
