using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.ViewModelTests
{
  [Serializable]
  public class TestBindingList :
    BusinessListBase<TestBindingList, TestChild>
  {
    public TestBindingList()
    { }

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
