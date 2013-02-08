//-----------------------------------------------------------------------
// <copyright file="MockEditableChild.partial.cs" company="Marimer LLC">
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
  public partial class MockEditableChild
  {
    #region Factories
    internal static MockEditableChild Load(Guid Id, string name)
    {
      return Csla.DataPortal.FetchChild<MockEditableChild>(Id, name);
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

      //Csla.DataPortal.UpdateChild<GrandChildList>(GrandChildren);
      Csla.DataPortal.UpdateChild(GrandChildren);
    }

    #endregion
  }
}