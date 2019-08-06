using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.ViewModelTests
{
  [Serializable]
  public class TestBusinessBindingList :
    BusinessBindingListBase<TestBusinessBindingList, TestChild>
  {
    #region Factory Methods

    public static void NewEditableList(EventHandler<DataPortalResult<TestBusinessBindingList>> callback)
    {
      var portal = new Csla.DataPortal<TestBusinessBindingList>();
      portal.CreateCompleted += callback;
      portal.BeginCreate();
    }

    public TestBusinessBindingList()
    { }

    #endregion

    public static TestBusinessBindingList NewEditableList()
    {
      return DataPortal.Create<TestBusinessBindingList>();
    }
    #region Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;

      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
