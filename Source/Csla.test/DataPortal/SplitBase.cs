//-----------------------------------------------------------------------
// <copyright file="SplitBase.cs" company="Marimer LLC">
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
  public abstract class SplitBase<T> : Csla.BusinessBase<T>
      where T : SplitBase<T>
  {
    #region Business Methods

    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id, RelationshipTypes.PrivateField);
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

    [Create]
		protected void DataPortal_Create()
    {
      _id = 0;
      TestResults.Reinitialise();
      TestResults.Add("Split", "Created");
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      _id = criteria.Id;
      TestResults.Reinitialise();
      TestResults.Add("Split", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Reinitialise();
      TestResults.Add("Split", "Inserted");
    }

    [Update]
		protected void DataPortal_Update()
    {
      TestResults.Reinitialise();
      TestResults.Add("Split", "Updated");
    }

    [Delete]
		private void DataPortal_Delete(Criteria criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("Split", "Deleted");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Reinitialise();
      TestResults.Add("Split", "SelfDeleted");
    }

    #endregion

  }
}