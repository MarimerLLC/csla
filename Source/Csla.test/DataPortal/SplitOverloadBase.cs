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

    public static T NewObject(IDataPortal<T> dataPortal)
    {
      return dataPortal.Create();
    }
    public static T NewObjectWithCriteria(IDataPortal<T> dataPortal)
    {
      return dataPortal.Create(new Criteria1(0));
    }

    public static T GetObject(int id, IDataPortal<T> dataPortal)
    {
      return dataPortal.Fetch(new Criteria(id));
    }
    public static void DeleteObject(int id, IDataPortal<T> dataPortal)
    {
      dataPortal.Delete(new Criteria(id));
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

    [Create]
		protected void DataPortal_Create()
    {
      _id = 0;
      TestResults.Reinitialise();
      TestResults.Add("SplitOverload", "Created");
      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(Criteria1 criteria)
    {
      _id = 0;
      TestResults.Reinitialise();
      TestResults.Add("SplitOverload", "Created1");
      BusinessRules.CheckRules();
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      _id = criteria.Id;
      TestResults.Reinitialise();
      TestResults.Add("SplitOverload", "Fetched");
    }
    private void DataPortal_Fetch(Criteria1 criteria)
    {
      _id = criteria.Id;
      TestResults.Reinitialise();
      TestResults.Add("SplitOverload", "Fetched1");
    }
    [Delete]
		private void DataPortal_Delete(Criteria criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("SplitOverload", "Deleted");
    }
    [Delete]
		private void DataPortal_Delete(Criteria1 criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("SplitOverload", "Deleted1");
    }
    #endregion

  }
}