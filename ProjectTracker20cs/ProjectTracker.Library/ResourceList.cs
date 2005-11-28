using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : ReadOnlyListBase<ResourceList, ResourceList.ResourceInfo>
  {

    #region ResourceInfo Class

    [Serializable()]
    public class ResourceInfo
    {

      private string _id;
      private string _name;

      public string Id
      {
        get { return _id; }
        set { _id = value; }
      }

      public string Name
      {
        get { return _name; }
        set { _name = value; }
      }

      public override bool Equals(object obj)
      {
        if (obj is ResourceInfo)
          return ((ResourceInfo)obj).Id == _id;
        else
          return false;
      }

      public override string ToString()
      {
        return Name;
      }

      public override int GetHashCode()
      {
        return _id.GetHashCode();
      }
    }

    #endregion

    #region Shared Methods

    public static ResourceList EmptyList()
    {
      return new ResourceList();
    }

    public static ResourceList GetResourceList()
    {
      return DataPortal.Fetch<ResourceList>(new Criteria());
    }

    [Serializable()]
    private class Criteria
    {
      // no criteria - we retrieve all resources
    }

    private ResourceList()
    {
      // prevent direct creation
    }

    #endregion

    #region Data Access

    protected override void DataPortal_Fetch(object criteria)
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getResources";

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            IsReadOnly = false;
            while (dr.Read())
            {
              ResourceInfo info = new ResourceInfo();
              info.Id = dr.GetString("id");
              info.Name = string.Format("{0}, {1}", dr.GetString("LastName"), dr.GetString("FirstName"));
              this.Add(info);
            }
            IsReadOnly = false;
            dr.Close();
          }
        }
        cn.Close();
      }
    }

    #endregion

  }
}
