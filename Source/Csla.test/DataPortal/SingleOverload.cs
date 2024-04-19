//-----------------------------------------------------------------------
// <copyright file="SingleOverload.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

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

    public static SingleOverload NewObject(IDataPortal<SingleOverload> dataPortal)
    {
      return dataPortal.Create();
    }
    public static SingleOverload NewObjectWithCriteria(IDataPortal<SingleOverload> dataPortal)
    {
      return dataPortal.Create(new OtherCriteria(0));
    }

    public static SingleOverload GetObject(int id, IDataPortal<SingleOverload> dataPortal)
    {
      return dataPortal.Fetch(new Criteria(id));
    }

    public static void DeleteObject(int id, IDataPortal<SingleOverload> dataPortal)
    {
      dataPortal.Delete(new Criteria(id));
    }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      public int Id { get; }

      public Criteria(int id)
      { Id = id; }
    }

    [Serializable()]
    private class OtherCriteria
    {
      public int Id { get; }

      public OtherCriteria(int id)
      { Id = id; }
    }

    [Create]
    protected void DataPortal_Create()
    {
      using (BypassPropertyChecks)
        Id = 0;
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Created0");
      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(Criteria criteria)
    {
      using (BypassPropertyChecks)
        Id = 0;
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Created");
      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(OtherCriteria criteria)
    {
      using (BypassPropertyChecks)
        Id = 0;
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Created1");
      BusinessRules.CheckRules();
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      using (BypassPropertyChecks)
        Id = criteria.Id;
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Fetched");
    }
    private void DataPortal_Fetch(OtherCriteria criteria)
    {
      using (BypassPropertyChecks)
        Id = criteria.Id;
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Fetched1");
    }
    [Delete]
    private void DataPortal_Delete(Criteria criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Deleted");
    }
    [Delete]
    private void DataPortal_Delete(OtherCriteria criteria)
    {
      TestResults.Reinitialise();
      TestResults.Add("SingleOverload", "Deleted1");
    }
    #endregion
  }
}