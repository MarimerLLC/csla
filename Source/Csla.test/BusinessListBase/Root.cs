//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.BusinessListBase
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    private static PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(c => c.Children);
    public ChildList Children
    {
      get { return GetProperty(ChildrenProperty); }
      private set { LoadProperty(ChildrenProperty, value); }
    }

    protected override void DataPortal_Create()
    {
      Children = Csla.DataPortal.CreateChild<ChildList>();
      base.DataPortal_Create();
    }

    protected override void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
    }

    protected override void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
    }

    protected override void DataPortal_DeleteSelf()
    {
      Children = Csla.DataPortal.CreateChild<ChildList>();
    }
  }
}