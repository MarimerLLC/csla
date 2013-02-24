﻿//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
    public static Child NewChild()
    {
      return Csla.DataPortal.CreateChild<Child>();
    }

    public static Child GetChild()
    {
      return Csla.DataPortal.FetchChild<Child>();
    }

    private Child()
    {
      MarkAsChild();
    }

    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(typeof(Child), new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty<string>(DataProperty); }
      set { SetProperty<string>(DataProperty, value); }
    }

    private static PropertyInfo<string> RootDataProperty = RegisterProperty<string>(typeof(Child), new PropertyInfo<string>("RootData","RootData",string.Empty));
    public string RootData
    {
      get { return GetProperty<string>(RootDataProperty); }
      set { SetProperty<string>(RootDataProperty, value); }
    }

    private string _status;
    public string Status
    {
      get { return _status; }
    }

    public void DeleteChild()
    {
      base.MarkDeleted();
    }

    protected void Child_Create()
    {
      _status = "Created";
    }

    protected void Child_Fetch()
    {
      _status = "Fetched";
    }

    protected void Child_Insert()
    {
      _status = "Inserted";
      if (this.Parent is ChildList)
      {
        LoadProperty(RootDataProperty, ((Root)((ChildList)this.Parent).MyParent).Data);
      }
      else
      {
        LoadProperty(RootDataProperty, ((Root)this.Parent).Data);
      }
    }

    protected void Child_Update()
    {
      _status = "Updated";
      if (this.Parent is ChildList)
      {
        LoadProperty(RootDataProperty, ((Root)((ChildList)this.Parent).MyParent).Data);
      }
      else
      {
        LoadProperty(RootDataProperty, ((Root)this.Parent).Data);
      }
    }

    protected void Child_DeleteSelf()
    {
      _status = "Deleted";
    }
  }
}