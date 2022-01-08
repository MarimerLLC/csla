//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.DataPortalChild
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    public Child()
    {
      MarkAsChild();
    }

    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(typeof(Child), new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty<string>(DataProperty); }
      set { SetProperty<string>(DataProperty, value); }
    }

    private static PropertyInfo<string> RootDataProperty = RegisterProperty<string>(typeof(Child), new PropertyInfo<string>("RootData", string.Empty));
    public string RootData
    {
      get { return GetProperty<string>(RootDataProperty); }
      set { SetProperty<string>(RootDataProperty, value); }
    }

    private static PropertyInfo<string> StatusProperty = RegisterProperty<string>(c => c.Status);
    public string Status
    {
      get { return GetProperty(StatusProperty); }
      private set { SetProperty(StatusProperty, value); }
    }

    public void DeleteChild()
    {
      base.MarkDeleted();
    }

    [CreateChild]
    protected override void Child_Create()
    {
      LoadProperty(StatusProperty, "Created");
    }

    [FetchChild]
    protected void Child_Fetch()
    {
      LoadProperty(StatusProperty, "Fetched");
    }

    [InsertChild]
    protected void Child_Insert(Root parent)
    {
      LoadProperty(StatusProperty, "Inserted");
      if (this.Parent is ChildList)
      {
        LoadProperty(RootDataProperty, ((Root)((ChildList)this.Parent).MyParent).Data);
      }
      else
      {
        LoadProperty(RootDataProperty, ((Root)this.Parent).Data);
      }
    }

    [UpdateChild]
    protected void Child_Update(Root parent)
    {
      LoadProperty(StatusProperty, "Updated");
      if (this.Parent is ChildList)
      {
        LoadProperty(RootDataProperty, ((Root)((ChildList)this.Parent).MyParent).Data);
      }
      else
      {
        LoadProperty(RootDataProperty, ((Root)this.Parent).Data);
      }
    }

    [DeleteSelfChild]
    protected void Child_DeleteSelf()
    {
      LoadProperty(StatusProperty, "Deleted");
    }
  }
}