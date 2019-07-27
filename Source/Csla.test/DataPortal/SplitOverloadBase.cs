//-----------------------------------------------------------------------
// <copyright file="SplitOverloadBase.cs" company="Marimer LLC">
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
  public abstract class SplitOverloadBase<T> : Csla.BusinessBase<T>
      where T : SplitOverloadBase<T>
  {
    #region Business Methods

    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    private int _id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _id); }
      set { SetProperty(IdProperty, ref _id, value); }
    }

    #endregion


    #region Factory Methods

    public static T NewObject()
    {
      return Csla.DataPortal.Create<T>();
    }
    public static T NewObjectWithCriteria()
    {
      return Csla.DataPortal.Create<T>(new Criteria1(0));
    }

    public static T GetObject(int id)
    {
      return Csla.DataPortal.Fetch<T>(new Criteria(id));
    }
    public static void DeleteObject(int id)
    {
      Csla.DataPortal.Delete<T>(new Criteria(id));
    }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria : CriteriaBase<Criteria>
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
    private class Criteria1 : CriteriaBase<Criteria1>
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }
      public Criteria1(int id)
      { _id = id; }
    }

    protected override void DataPortal_Create()
    {
      _id = 0;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SplitOverload", "Created");
    }
    private void DataPortal_Create(Criteria1 criteria)
    {
      _id = 0;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SplitOverload", "Created1");
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      _id = criteria.Id;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SplitOverload", "Fetched");
    }
    private void DataPortal_Fetch(Criteria1 criteria)
    {
      _id = criteria.Id;
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SplitOverload", "Fetched1");
    }
    private void DataPortal_Delete(Criteria criteria)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SplitOverload", "Deleted");
    }
    private void DataPortal_Delete(Criteria1 criteria)
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext.Add("SplitOverload", "Deleted1");
    }
    #endregion

  }
}