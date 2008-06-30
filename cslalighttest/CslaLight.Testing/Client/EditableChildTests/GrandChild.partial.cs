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
  public partial class GrandChild
  {
    #region Factories

    internal static GrandChild Load(int id, Guid parentId, string name)
    {
      ChildDataPortal<GrandChild> dp = new ChildDataPortal<GrandChild>();
      return (GrandChild)dp.Fetch(id, parentId, name);
    }

    #endregion

    #region Data Access

    public void Child_Fetch(int id, Guid parentId, string name)
    {
      LoadProperty<int>(IdProperty, id);
      LoadProperty<Guid>(ParentIdProperty, parentId);
      LoadProperty<string>(NameProperty, name);
    }

    #endregion
  }
}
