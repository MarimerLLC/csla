//-----------------------------------------------------------------------
// <copyright file="ERitem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.EditableRootList
{
  [Serializable]
  public class ERitem : BusinessBase<ERitem>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public ERitem()
    { }

    private ERitem(string data)
    {
      using (BypassPropertyChecks)
        Data = data;
      MarkOld();
    }

    public static ERitem NewItem()
    {
      return new ERitem();
    }

    public static ERitem GetItem(string data)
    {
      return new ERitem(data);
    }

    protected override void DataPortal_Insert()
    {
      ApplicationContext.GlobalContext["DP"] = "Insert";
    }

    protected override void DataPortal_Update()
    {
      ApplicationContext.GlobalContext["DP"] = "Update";
    }

    protected override void DataPortal_DeleteSelf()
    {
      ApplicationContext.GlobalContext["DP"] = "DeleteSelf";
    }

    protected void DataPortal_Delete(object criteria)
    {
      ApplicationContext.GlobalContext["DP"] = "Delete";
    }
  }
}