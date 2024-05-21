//-----------------------------------------------------------------------
// <copyright file="LegacySplitBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.DataPortalTest
{
  [Serializable]
  public abstract class LegacySplitBase<T> : BusinessBase<T>
      where T : LegacySplitBase<T>
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

    #region Data Access

    [Serializable]
    internal class Criteria : CriteriaBase<Criteria>
    {
      public int Id { get; }

      public Criteria(int id)
      { Id = id; }
    }

    [Create]
    protected void DataPortal_Create()
    {
      _id = 0;
      TestResults.Reinitialise();
      TestResults.Add("LegacySplit", "Created");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      _id = ((Criteria)criteria).Id;
      TestResults.Reinitialise();
      TestResults.Add("LegacySplit", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Reinitialise();
      TestResults.Add("LegacySplit", "Inserted");
    }

    [Update]
    protected void DataPortal_Update()
    {
      TestResults.Reinitialise();
      TestResults.Add("LegacySplit", "Updated");
    }

    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("LegacySplit", "Deleted");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Reinitialise();
      TestResults.Add("LegacySplit", "SelfDeleted");
    }

    #endregion

  }
}