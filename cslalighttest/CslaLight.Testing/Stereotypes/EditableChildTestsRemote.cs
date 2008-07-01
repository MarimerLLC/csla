using cslalighttest.Engine;
using Csla;
using Csla.Testing.Business.EditableChildTests;
using cslalighttest.Properties;
using System;

namespace cslalighttest.Stereotypes
{
  [TestClass]
  public class EditableChildTestsRemote
  {
    [TestSetup]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = Resources.RemotePortalUrl;
    }

    [TestMethod]
    public void ListWithChildrenNoCriteria(AsyncTestContext context)
    {
      MockList.FetchAll((o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(3, e.Object.Count);

        context.Assert.AreEqual(MockList.MockEditableChildId1, e.Object[0].Id);
        context.Assert.AreEqual("Child_Fetch", e.Object[0].DataPortalMethod);
        context.Assert.IsNotNull(e.Object[0].GrandChildren);
        context.Assert.AreEqual(1, e.Object[0].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId1, e.Object[0].GrandChildren[0].Id);
        context.Assert.AreEqual("Child_Fetch", e.Object[0].GrandChildren[0].DataPortalMethod);

        context.Assert.AreEqual(MockList.MockEditableChildId2, e.Object[1].Id);
        context.Assert.AreEqual("Child_Fetch", e.Object[1].DataPortalMethod);
        context.Assert.IsNotNull(e.Object[1].GrandChildren);
        context.Assert.AreEqual(1, e.Object[1].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId2, e.Object[1].GrandChildren[0].Id);
        context.Assert.AreEqual("Child_Fetch", e.Object[1].GrandChildren[0].DataPortalMethod);

        context.Assert.AreEqual(MockList.MockEditableChildId3, e.Object[2].Id);
        context.Assert.AreEqual("Child_Fetch", e.Object[2].DataPortalMethod);
        context.Assert.IsNotNull(e.Object[2].GrandChildren);
        context.Assert.AreEqual(1, e.Object[2].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId3, e.Object[2].GrandChildren[0].Id);
        context.Assert.AreEqual("Child_Fetch", e.Object[2].GrandChildren[0].DataPortalMethod);

        context.Assert.Success();
      });
    }

    [TestMethod]
    public void ListWithChildrenCriteria(AsyncTestContext context)
    {
      MockList.FetchByName("c2", (o, e) =>
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.AreEqual(1, e.Object.Count);
        context.Assert.AreEqual(MockList.MockEditableChildId2, e.Object[0].Id);
        Assert.AreEqual("Child_Fetch", e.Object[0].DataPortalMethod);
        context.Assert.IsFalse(e.Object[0].IsNew);
        context.Assert.IsFalse(e.Object[0].IsDirty);

        context.Assert.AreEqual(1, e.Object[0].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId2, e.Object[0].GrandChildren[0].Id);
        Assert.AreEqual("Child_Fetch", e.Object[0].GrandChildren[0].DataPortalMethod);
        context.Assert.IsFalse(e.Object[0].GrandChildren[0].IsNew);
        context.Assert.IsFalse(e.Object[0].GrandChildren[0].IsDirty);

        context.Assert.Success();
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
      MockList.FetchByName("c2", (o, e) => 
      {
        context.Assert.IsNull(e.Error);
        context.Assert.IsNotNull(e.Object);
        context.Assert.Try(() => e.Object[0].GrandChildren.Save());
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
          context.Assert.IsNotNull(e2.NewObject);
          MockList l2 = (MockList)e2.NewObject;

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
