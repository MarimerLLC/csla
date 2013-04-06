using System;
using System.Linq;

#if SILVERLIGHT
using Csla.DataPortalClient;
using Csla.Serialization;
#endif


namespace Csla.Testing.Business.EditableChildTests
{
  [Serializable]
  public partial class GrandChildList : BusinessListBase<GrandChildList, GrandChild>
  {
    #region  Data Access

    public static int GrandChildId1 = 1;
    public static int GrandChildId2 = 2;
    public static int GrandChildId3 = 3;

#if SILVERLIGHT
    public void Child_Fetch(Guid parentId)
#else
    private void Child_Fetch(Guid parentId)
#endif
    {
      RaiseListChangedEvents = false;

      var grandChildren = 
        new[]
          {
            GrandChild.Load(GrandChildId1, MockList.MockEditableChildId1, "gc1"), 
            GrandChild.Load(GrandChildId2, MockList.MockEditableChildId2, "gc2"), 
            GrandChild.Load(GrandChildId3, MockList.MockEditableChildId3, "gc3")
          };

      var found = from c in grandChildren
                  where parentId == Guid.Empty || c.ParentId == parentId
                  select c;

      AddRange(found.ToArray());

      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
