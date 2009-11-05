using System;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class GrandChildList
  {
    private GrandChildList() { }

    #region Server Factories

    public static GrandChildList Load(Guid parentId)
    {
      return Csla.DataPortal.FetchChild<GrandChildList>(parentId);
    }

    #endregion

  }
}
