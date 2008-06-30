using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.DataPortalClient;
using Csla.Serialization;

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

      GrandChild gc1 = GrandChild.Load(GrandChildId1, MockList.MockEditableChildId1, "gc1");
      GrandChild gc2 = GrandChild.Load(GrandChildId2, MockList.MockEditableChildId2, "gc2");
      GrandChild gc3 = GrandChild.Load(GrandChildId3, MockList.MockEditableChildId3, "gc3");
      GrandChild[] grandChildren = new GrandChild[] { gc1, gc2, gc3 };

      var found = from c in grandChildren
                  where parentId == Guid.Empty || c.ParentId == parentId
                  select c;

      AddRange(found.ToArray());

      RaiseListChangedEvents = true;
    }

    protected override void DataPortal_Update()
    {
      Child_Update();
    }

    #endregion
  }
}
