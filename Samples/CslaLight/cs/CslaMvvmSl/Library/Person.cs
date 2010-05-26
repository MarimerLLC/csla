using System;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Library
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    #region Factory Methods

    public static void NewPerson(EventHandler<DataPortalResult<Person>> callback)
    {
      var dp = new DataPortal<Person>();
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    public static void GetPerson(int id, EventHandler<DataPortalResult<Person>> callback)
    {
      var dp = new DataPortal<Person>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(id);
    }

    #endregion

    #region Data Access

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void Child_Fetch(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void DataPortal_Fetch(int id, Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = "Rocky";
      }
      handler(this, null);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      handler(this, null);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Update(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      handler(this, null);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_DeleteSelf(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      handler(this, null);
    }

    #endregion
  }
}
