using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.EditableChildTests
{
  partial class MockEditableChild
  {
    #region Server Factories

    public static MockEditableChild LoadMockEditableChild(Guid Id, string name)
    {
      return DataPortal.FetchChild<MockEditableChild>(Id, name);
    }

    #endregion

    #region Data Access

    private void Child_Fetch(Guid id, string name)
    {
      LoadProperty<Guid>(IdProperty, id);
      LoadProperty<string>(NameProperty, name);
    }

    #endregion
  }
}
