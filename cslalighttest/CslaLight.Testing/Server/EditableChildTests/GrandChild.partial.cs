using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class GrandChild
  {
    #region Factories

    internal static GrandChild Load(int id, Guid parentId, string name)
    {
      return DataPortal.FetchChild<GrandChild>(id, parentId, name);
    }

    #endregion

    #region Data Access

    private void Child_Fetch(int id, Guid parentId, string name)
    {
      LoadProperty<int>(IdProperty, id);
      LoadProperty<Guid>(ParentIdProperty, parentId);
      LoadProperty<string>(NameProperty, name);

      LoadProperty<string>(DataPortalMethodProperty, "Child_Fetch");
    }

    private void Child_Update()
    {
      LoadProperty<string>(DataPortalMethodProperty, "Child_Update");
    }
    #endregion
  }
}
