using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.ViewModelTests
{
  [Serializable]
  public class TestBindingList :
    BusinessListBase<TestBindingList, TestChild>
  {
    #region Factory Methods

    public static void NewEditableList(EventHandler<DataPortalResult<TestBindingList>> callback)
    {
      var portal = new Csla.DataPortal<TestBindingList>();
      portal.CreateCompleted += callback;
      portal.BeginCreate();
    }

    public TestBindingList()
    { }

    #endregion

    public static TestBindingList NewEditableList()
    {
      return DataPortal.Create<TestBindingList>();
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
