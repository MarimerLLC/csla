using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceList : ReadOnlyCollectionBase
  {
    #region Data Structure

    [Serializable()]
      public struct ResourceInfo
    {
      // this has private members, public properties because
      // ASP.NET can't databind to public members of a structure...
      private string _id;
      private string _name;

      public string ID
      {
        get
        {
          return _id;
        }
        set
        {
          _id = value;
        }
      }

      public string Name
      {
        get
        {
          return _name;
        }
        set
        {
          _name = value;
        }
      }

      public bool Equals(ResourceInfo info)
      {
        return _id == info.ID;
      }
    }

    #endregion

    #region Business Properties and Methods

    public ResourceInfo this [int index]
    {
      get
      {
        return (ResourceInfo)List[index];
      }
    }

    #endregion

    #region Contains

    public bool Contains(ResourceInfo item)
    {
      foreach(ResourceInfo child in List)
        if(child.Equals(item))
          return true;
      return false;
    }

    #endregion

    #region Static Methods

    public static ResourceList EmptyList()
    {
      return new ResourceList();
    }

    public static ResourceList GetResourceList() 
    {
      return (ResourceList)DataPortal.Fetch(new Criteria());
    }

    #endregion

    #region Criteria 

    [Serializable()]
      private class Criteria
    {
      // no criteria - we retrieve all resources
    }

    #endregion

    #region Constructors

    private ResourceList()
    {
      // prevent direct creation
    }

    #endregion

    #region Data Access

    protected override void DataPortal_Fetch(object criteria)
    {
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        using(SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getResources";

          using(SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            locked = false;
            while(dr.Read())
            {
              ResourceInfo info = new ResourceInfo();
              info.ID = dr.GetString(0);
              info.Name = dr.GetString(1) + ", " + dr.GetString(2);
              List.Add(info);
            }
            locked = true;
          }
        }
      }
    }

    #endregion

  }
}
