using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.EditableChildTests
{
  partial class MockEditableChild
  {
    #region Server Factories

    public static MockEditableChild Load(Guid Id, string name)
    {
      return DataPortal.FetchChild<MockEditableChild>(Id, name);
    }

    #endregion

    #region Data Access

    private void Child_Fetch(Guid id, string name)
    {
      LoadProperty<Guid>(IdProperty, id);
      LoadProperty<string>(NameProperty, name);
      LoadProperty<string>(DataPortalMethodProperty, "Child_Fetch");

      LoadProperty<GrandChildList>(GrandChildrenProperty, GrandChildList.Load(id));
    }
    
    private void Child_Update()
    {
      LoadProperty<string>(DataPortalMethodProperty, "Child_Update");

      DataPortal.UpdateChild(GrandChildren);
    }

    #endregion
  }
}
