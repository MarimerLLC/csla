using System;
using Csla;
using Csla.Serialization;

namespace Library
{
  [Serializable]
  public class PersonList : BusinessListBase<PersonList, Person>
  {
    #region Factory Methods

    public static void GetPersonList(EventHandler<DataPortalResult<PersonList>> callback)
    {
      //var dp = new DataPortal<PersonList>();
      //dp.FetchCompleted += callback;
      //dp.BeginFetch();

      DataPortal.BeginFetch<PersonList>(callback);
    }

    #endregion

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void DataPortal_Fetch(Csla.DataPortalClient.LocalProxy<PersonList>.CompletedHandler handler)
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      try
      {
        foreach (var item in MockDb.Persons)
          Add(DataPortal.FetchChild<Person>(item.Id, item.FirstName + " " + item.LastName));
        handler(this, null);
      }
      catch (Exception ex)
      {
        handler(this, ex);
      }
      finally
      {
        RaiseListChangedEvents = rlce;
      }
    }

    public override void DataPortal_Update(Csla.DataPortalClient.LocalProxy<PersonList>.CompletedHandler handler)
    {
      try
      {
        base.Child_Update();
        handler(this, null);
      }
      catch (Exception ex)
      {
        handler(this, ex);
      }
    }
  }
}
