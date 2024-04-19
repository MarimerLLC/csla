//-----------------------------------------------------------------------
// <copyright file="Legacy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.DataPortalTest
{
  [Serializable()]
  class Legacy : BusinessBase<Legacy>
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

    [Serializable()]
    internal class Criteria
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
      TestResults.Add("Legacy", "Created");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      _id = ((Criteria)criteria).Id;
      TestResults.Reinitialise();
      TestResults.Add("Legacy", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Reinitialise();
      TestResults.Add("Legacy", "Inserted");
    }

    [Update]
    protected void DataPortal_Update()
    {
      TestResults.Reinitialise();
      TestResults.Add("Legacy", "Updated");
    }

    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("Legacy", "Deleted");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Reinitialise();
      TestResults.Add("Legacy", "SelfDeleted");
    }

    #endregion
  }
}