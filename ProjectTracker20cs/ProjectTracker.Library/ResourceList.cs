using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : 
    ReadOnlyListBase<ResourceList, ResourceList.ResourceInfo>
  {
    #region ResourceInfo Class

    [Serializable()]
    public class ResourceInfo : 
      ReadOnlyBase<ResourceInfo>
    {
      #region Business Methods

      private int _id;
      private string _name;

      public int Id
      {
        get { return _id; }
      }

      public string Name
      {
        get { return _name; }
      }

      protected override object GetIdValue()
      {
        return _id;
      }

      #endregion

      #region Constructors

      private ResourceInfo()
      { /* require use of factory methods */ }

      internal ResourceInfo(int id, string name)
      {
        _id = id;
        _name = name;
      }

      #endregion
    }

    #endregion

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
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
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
              ResourceInfo info = new ResourceInfo(
                dr.GetInt32("Id"),
                string.Format("{0}, {1}", 
                  dr.GetString("LastName"), 
                  dr.GetString("FirstName")));
              this.Add(info);
            }
            IsReadOnly = false;
          }
        }
      }
    }

    #endregion

  }
}
