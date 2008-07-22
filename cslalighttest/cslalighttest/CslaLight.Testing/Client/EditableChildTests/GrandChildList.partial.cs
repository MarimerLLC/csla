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
  public partial class GrandChildList
  {
    #region Factories

    internal static GrandChildList Load(Guid parentId)
    {
      ChildDataPortal<GrandChildList> dp = new ChildDataPortal<GrandChildList>();
      return (GrandChildList)dp.Fetch(parentId);
    }

    #endregion

  }
}
