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

    public static StronglyTypedDP NewStronglyTypedDP()
    {
      return Csla.DataPortal.Create<StronglyTypedDP>(new StronglyTypedDP.Criteria());
    }

    public static StronglyTypedDP GetStronglyTypedDP(int id)
    {
      return Csla.DataPortal.Fetch<StronglyTypedDP>(new StronglyTypedDP.Criteria(id));
    }

    public static void DeleteStronglyTypedDP(int id)
    {
      Csla.DataPortal.Delete<StronglyTypedDP>(new Criteria(id));
    }

    protected void DataPortal_Create(StronglyTypedDP.Criteria criteria)
    {
      using (BypassPropertyChecks)
      {
        Data = criteria._data;
        Id = criteria._ID;
      }
      Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Created");
    }

    protected void DataPortal_Fetch(StronglyTypedDP.Criteria criteria)
    {
      using (BypassPropertyChecks)
      {
        Data = criteria._data;
        Id = criteria._ID;
      }
      MarkOld();
      Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Fetched");
    }

    protected override void DataPortal_Insert()
    {
      Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Inserted");
    }

    protected override void DataPortal_Update()
    {
      Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Updated");
    }

    protected override void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Deleted self");
    }

    protected void DataPortal_Delete(StronglyTypedDP.Criteria criteria)
    {
      Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP_Criteria", criteria._ID);
    }
  }
}