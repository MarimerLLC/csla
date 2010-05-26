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
      var dp = new DataPortal<PersonList>();
      dp.FetchCompleted += callback;
      dp.BeginFetch();
    }

    #endregion

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void DataPortal_Fetch(Csla.DataPortalClient.LocalProxy<PersonList>.CompletedHandler handler)
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;

      Add(DataPortal.FetchChild<Person>(31, "Amr"));
      Add(DataPortal.FetchChild<Person>(2, "Telaa"));
      Add(DataPortal.FetchChild<Person>(33, "Seww"));
      Add(DataPortal.FetchChild<Person>(44, "Klew"));
      Add(DataPortal.FetchChild<Person>(15, "Aej"));
      Add(DataPortal.FetchChild<Person>(6, "Uiz"));

      RaiseListChangedEvents = rlce;

      handler(this, null);
    }
  }
}
