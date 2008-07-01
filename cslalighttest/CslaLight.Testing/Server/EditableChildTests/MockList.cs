using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableChildTests
{
  [Serializable]
  public partial class MockList : BusinessListBase<MockList, MockEditableChild>
  {
    #region  Data Access

    public static Guid MockEditableChildId1 = new Guid("{4AF07634-1D10-4d09-BBCB-42396324C103}");
    public static Guid MockEditableChildId2 = new Guid("{402DB94B-0D93-4be2-AFD0-7D1587CDDBC2}");
    public static Guid MockEditableChildId3 = new Guid("{B2ADE99D-7AB4-4e70-AEA1-D70443BCC3B0}");

#if SILVERLIGHT
    private void Fetch(LocalProxy<MockList>.CompletedHandler completed, string nameFilter)
#else
    private void Fetch(string nameFilter)
#endif
    {
      RaiseListChangedEvents = false;

      MockEditableChild c1 = MockEditableChild.Load(MockEditableChildId1, "c1");
      MockEditableChild c2 = MockEditableChild.Load(MockEditableChildId2, "c2");
      MockEditableChild c3 = MockEditableChild.Load(MockEditableChildId3, "c3");
      MockEditableChild[] children = new MockEditableChild[] { c1, c2, c3 };

      var found = from c in children
                  where string.IsNullOrEmpty(nameFilter) || c.Name == nameFilter
                  select c;

      AddRange(found.ToArray());

      RaiseListChangedEvents = true;

#if SILVERLIGHT
      completed(this, null);
#endif
    }

    #endregion
  }
}
