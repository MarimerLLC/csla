using Csla;
using Csla.Data;
using System;
using System.Linq;

namespace WpfDesignTime.Business
{
  [Serializable()]
  public class ResourceList : ReadOnlyListBase<ResourceList, ResourceInfo>
  {
    #region  Factory Methods

    public static ResourceList EmptyList()
    {
      return new ResourceList();
    }

    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>();
    }

    private ResourceList()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      base.IsReadOnly = false;
      // Simulated Database access
      for (int i = 1; i < 10; i++)
      {
        this.Add(new ResourceInfo(i, "Last Name # " + i.ToString(), "First Name # " + i.ToString()));
      }

      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
    
    #endregion

    #region  Design Time Support
    private void DesignTime_Create()
    {
      IsReadOnly = false;
      this.Add(new ResourceInfo(1, "Doe", "John"));
      this.Add(new ResourceInfo(2, "Doe", "Jane"));
      IsReadOnly = true;
    }
    #endregion

  }
}