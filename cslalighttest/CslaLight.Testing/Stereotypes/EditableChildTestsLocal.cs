
using cslalighttest.Engine;
using Csla;
using Csla.Testing.Business.EditableChildTests;
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
      MockList.FetchAll((l, e) =>
      {
        Assert.IsNull(e);
        Assert.IsNotNull(l);
        Assert.AreEqual(3, l.Count);
        Assert.AreEqual(MockList.MockEditableChildId1, l[0].Id);
        Assert.AreEqual(MockList.MockEditableChildId2, l[1].Id);
        Assert.AreEqual(MockList.MockEditableChildId3, l[2].Id);
      });
    }

    [TestMethod]
    public void ListWIthChildrenCriteria()
    {
      MockList.FetchByName("c2", (l, e) =>
      {
        Assert.IsNull(e);
        Assert.IsNotNull(l);
        Assert.AreEqual(1, l.Count);
        Assert.AreEqual(MockList.MockEditableChildId2, l[0].Id);
        Assert.IsFalse(l[0].IsNew);
        Assert.IsFalse(l[0].IsDirty);
      });
    }
  }
}
