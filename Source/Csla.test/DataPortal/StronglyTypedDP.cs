//-----------------------------------------------------------------------
// <copyright file="StronglyTypedDP.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortal
{
  [Serializable()]
  public class StronglyTypedDP : BusinessBase<StronglyTypedDP>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    //criteria class needs to be protected since DataPortal_xyz methods are 
    //protected
    [Serializable()]
    protected class Criteria
    {
      public string _data;
      public int _ID;

      public Criteria()
      {
        _data = "new default data";
        _ID = 56;
      }

      public Criteria(int id)
      {
        this._ID = id;
        this._data = "fetched existing data";
      }
    }

    public static StronglyTypedDP NewStronglyTypedDP(IDataPortal<StronglyTypedDP> dataPortal)
    {
      return dataPortal.Create(new StronglyTypedDP.Criteria());
    }

    public static StronglyTypedDP GetStronglyTypedDP(int id, IDataPortal<StronglyTypedDP> dataPortal)
    {
      return dataPortal.Fetch(new StronglyTypedDP.Criteria(id));
    }

    public static void DeleteStronglyTypedDP(int id, IDataPortal<StronglyTypedDP> dataPortal)
    {
      dataPortal.Delete(new Criteria(id));
    }

    protected void DataPortal_Create(StronglyTypedDP.Criteria criteria)
    {
      using (BypassPropertyChecks)
      {
        Data = criteria._data;
        Id = criteria._ID;
      }
      TestResults.Add("StronglyTypedDP", "Created");
    }

    protected void DataPortal_Fetch(StronglyTypedDP.Criteria criteria)
    {
      using (BypassPropertyChecks)
      {
        Data = criteria._data;
        Id = criteria._ID;
      }
      MarkOld();
      TestResults.Add("StronglyTypedDP", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Add("StronglyTypedDP", "Inserted");
    }

    [Update]
		protected void DataPortal_Update()
    {
      TestResults.Add("StronglyTypedDP", "Updated");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Add("StronglyTypedDP", "Deleted self");
    }

    [Delete]
	protected void DataPortal_Delete(StronglyTypedDP.Criteria criteria)
    {
      TestResults.Add("StronglyTypedDP_Criteria", criteria._ID.ToString());
    }
  }
}