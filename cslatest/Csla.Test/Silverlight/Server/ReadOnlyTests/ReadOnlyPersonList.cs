using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core.FieldManager;
using System.Linq;
using System.Text;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.ReadOnlyTest
{
  [Serializable]
  public class ReadOnlyPersonList : ReadOnlyListBase<ReadOnlyPersonList, ReadOnlyPerson>
  {


    public static void Fetch(EventHandler<DataPortalResult<ReadOnlyPersonList>> completed)
    {
      DataPortal<ReadOnlyPersonList> dp = new DataPortal<ReadOnlyPersonList>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }

#if SILVERLIGHT
    public void DataPortal_Fetch(LocalProxy<ReadOnlyPersonList>.CompletedHandler completed)
    {
      Fetch(completed);
    }
#else
    private void DataPortal_Fetch()
    {
      Fetch();
    }
#endif
#if SILVERLIGHT
    private void Fetch(LocalProxy<ReadOnlyPersonList>.CompletedHandler completed)
#else
    private void Fetch()
#endif
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      Add(ReadOnlyPerson.GetReadOnlyPersonForList("John Doe", 1981));
      Add(ReadOnlyPerson.GetReadOnlyPersonForList("Jane Doe", 1982));
      IsReadOnly = true;
      RaiseListChangedEvents = true;

#if SILVERLIGHT
      completed(this, null);
#endif
    }
  }
}
