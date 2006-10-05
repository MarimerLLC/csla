using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : 
    ReadOnlyListBase<ResourceList, ResourceInfo>
  {
    #region Factory Methods

    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>(new Criteria());
    }

    private ResourceList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    { /* no criteria - retrieve all resources */ }

    private void DataPortal_Fetch(Criteria criteria)
    {
      this.RaiseListChangedEvents = false;
      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getResources";

          using (SafeDataReader dr = 
            new SafeDataReader(cm.ExecuteReader()))
          {
            IsReadOnly = false;
            while (dr.Read())
            {
              ResourceInfo info = new ResourceInfo(dr);
              this.Add(info);
            }
            IsReadOnly = true;
          }
        }
      }
      this.RaiseListChangedEvents = true;
    }

    #endregion

  }
}
