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

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Add("DP", "Insert");
    }

    [Update]
		protected void DataPortal_Update()
    {
      TestResults.Add("DP", "Update");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Add("DP", "DeleteSelf");
    }

    [Delete]
		protected void DataPortal_Delete(object criteria)
    {
      TestResults.Add("DP", "Delete");
    }
  }
}