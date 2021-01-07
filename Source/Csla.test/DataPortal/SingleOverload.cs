//-----------------------------------------------------------------------
// <copyright file="SingleOverload.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortalTest
{
  [Serializable()]
  class SingleOverload : BusinessBase<SingleOverload>
  {
    #region Business Methods

    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    #endregion

    #region Factory Methods

    public static SingleOverload NewObject()
    {
      return Csla.DataPortal.Create<SingleOverload>();
    }
    public static SingleOverload NewObjectWithCriteria()
    {
      return Csla.DataPortal.Create<SingleOverload>(new OtherCriteria(0));
    }

    public static SingleOverload GetObject(int id)
    {
      return Csla.DataPortal.Fetch<SingleOverload>(new Criteria(id));
    }

    public static void DeleteObject(int id)
    {
      Csla.DataPortal.Delete<SingleOverload>(new Criteria(id));
    }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }
      public Criteria(int id)
      { _id = id; }
    }

    [Serializable()]
    private class OtherCriteria
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }
      public OtherCriteria(int id)
      { _id = id; }
    }

    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
        Id = 0;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Created0");
    }

    private void DataPortal_Create(Criteria criteria)
    {
      using (BypassPropertyChecks)
        Id = 0;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Created");
    }

    private void DataPortal_Create(OtherCriteria criteria)
    {
      using (BypassPropertyChecks)
        Id = 0;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Created1");
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      using (BypassPropertyChecks)
        Id = criteria.Id;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Fetched");
    }
    private void DataPortal_Fetch(OtherCriteria criteria)
    {
      using (BypassPropertyChecks)
        Id = criteria.Id;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Fetched1");
    }
    private void DataPortal_Delete(Criteria criteria)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Deleted");
    }
    private void DataPortal_Delete(OtherCriteria criteria)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SingleOverload", "Deleted1");
    }
    #endregion
  }
}