using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class MockEditableChild
  {
    #region Factories
    internal static MockEditableChild LoadMockEditableChild(Guid Id, string name)
    {
      ChildDataPortal<MockEditableChild> dp = new ChildDataPortal<MockEditableChild>();
      return (MockEditableChild)dp.Fetch(Id, name);
    }
    #endregion

    #region Data Access

    public void Child_Fetch(Guid id, string name)
    {
      LoadProperty<Guid>(IdProperty, id);
      LoadProperty<string>(NameProperty, name);
    }

    #endregion
  }
}
