//-----------------------------------------------------------------------
// <copyright file="GrandChild.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
  public partial class GrandChild
  {
    #region Factories

    internal static GrandChild Load(int id, Guid parentId, string name)
    {
      return Csla.DataPortal.FetchChild<GrandChild>(id, parentId, name);
    }

    #endregion

    #region Data Access

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Child_Fetch(int id, Guid parentId, string name)
    {
      LoadProperty<int>(IdProperty, id);
      LoadProperty<Guid>(ParentIdProperty, parentId);
      LoadProperty<string>(NameProperty, name);

      LoadProperty<string>(DataPortalMethodProperty, "Child_Fetch");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Child_Update()
    {
      LoadProperty<string>(DataPortalMethodProperty, "Child_Update");
    }

    #endregion
  }
}