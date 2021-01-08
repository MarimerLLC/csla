//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;

namespace Csla.Test.FieldManager
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

    private string _status;
    public string Status
    {
      get { return _status; }
    }

    public void DeleteChild()
    {
      MarkDeleted();
    }

    protected override void Child_Create()
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
    }

    protected void Child_Update()
    {
      _status = "Updated";
    }

    protected void Child_DeleteSelf()
    {
      _status = "Deleted";
    }
  }
}