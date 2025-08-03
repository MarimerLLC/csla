//-----------------------------------------------------------------------
// <copyright file="MockEditableChild.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Testing.Business.EditableChildTests
{
  partial class MockEditableChild
  {
    #region Server Factories

    public static MockEditableChild Load(Guid Id, string name)
    {
      return Csla.DataPortal.FetchChild<MockEditableChild>(Id, name);
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

      Csla.DataPortal.UpdateChild(GrandChildren);
    }

    #endregion
  }
}