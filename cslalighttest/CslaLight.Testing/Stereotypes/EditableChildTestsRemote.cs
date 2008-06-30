using cslalighttest.Engine;
using Csla;
using Csla.Testing.Business.EditableChildTests;
using cslalighttest.Properties;

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
      MockList.FetchAll((l, e) =>
      {
        context.Assert.IsNull(e);
        context.Assert.IsNotNull(l);
        context.Assert.AreEqual(3, l.Count);
        context.Assert.AreEqual(MockList.MockEditableChildId1, l[0].Id);
        context.Assert.IsNotNull(l[0].GrandChildren);
        context.Assert.AreEqual(1, l[0].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId1, l[0].GrandChildren[0].Id);

        context.Assert.AreEqual(MockList.MockEditableChildId2, l[1].Id);
        context.Assert.IsNotNull(l[1].GrandChildren);
        context.Assert.AreEqual(1, l[1].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId2, l[1].GrandChildren[0].Id);

        context.Assert.AreEqual(MockList.MockEditableChildId3, l[2].Id);
        context.Assert.IsNotNull(l[2].GrandChildren);
        context.Assert.AreEqual(1, l[2].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId3, l[2].GrandChildren[0].Id);

        context.Assert.Success();
      });
    }

    [TestMethod]
    public void ListWIthChildrenCriteria(AsyncTestContext context)
    {
      MockList.FetchByName("c2", (l, e) =>
      {
        context.Assert.IsNull(e);
        context.Assert.IsNotNull(l);
        context.Assert.AreEqual(1, l.Count);
        context.Assert.AreEqual(MockList.MockEditableChildId2, l[0].Id);
        context.Assert.IsFalse(l[0].IsNew);
        context.Assert.IsFalse(l[0].IsDirty);

        context.Assert.AreEqual(1, l[0].GrandChildren.Count);
        context.Assert.AreEqual(GrandChildList.GrandChildId2, l[0].GrandChildren[0].Id);
        context.Assert.IsFalse(l[0].GrandChildren[0].IsNew);
        context.Assert.IsFalse(l[0].GrandChildren[0].IsDirty);

        context.Assert.Success();
      });
    }
  }
}
