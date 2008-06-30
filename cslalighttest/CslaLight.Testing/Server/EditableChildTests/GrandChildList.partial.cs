using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class GrandChildList
  {
    private GrandChildList() { }

    #region Server Factories

    public static GrandChildList Load(Guid parentId)
    {
      return DataPortal.FetchChild<GrandChildList>(parentId);
    }

    #endregion

  }
}
