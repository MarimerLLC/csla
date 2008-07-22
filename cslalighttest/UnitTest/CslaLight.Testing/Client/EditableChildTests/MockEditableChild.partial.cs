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
using System.ComponentModel;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class MockEditableChild
  {
    #region Factories
    internal static MockEditableChild Load(Guid Id, string name)
    {
      ChildDataPortal<MockEditableChild> dp = new ChildDataPortal<MockEditableChild>();
      return (MockEditableChild)dp.Fetch(Id, name);
    }
    #endregion

    #region Data Access
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Child_Fetch(Guid id, string name)
    {
      LoadProperty<Guid>(IdProperty, id);
      LoadProperty<string>(NameProperty, name);
      LoadProperty<string>(DataPortalMethodProperty, "Child_Fetch");

      LoadProperty<GrandChildList>(GrandChildrenProperty, GrandChildList.Load(id));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Child_Update()
    {
      LoadProperty<string>(DataPortalMethodProperty, "Child_Update");

      ChildDataPortal<GrandChildList> dp = new ChildDataPortal<GrandChildList>();
      dp.Update(GrandChildren);
    }

    #endregion
  }
}
