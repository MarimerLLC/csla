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
      private string _ID;
      private string _Name;

      public string ID
      {
        get
        {
          return _ID;
        }
        set
        {
          _ID = value;
        }
      }

      public string Name
      {
        get
        {
          return _Name;
        }
        set
        {
          _Name = value;
        }
      }

      public bool Equals(ResourceInfo info)
      {
        return _ID == info.ID;
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
      SqlConnection cn = new SqlConnection(DB("PTracker"));
      SqlCommand cm = new SqlCommand();

      cn.Open();
      try
      {
        cm.Connection = cn;
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "getResources";

        SafeDataReader dr = new SafeDataReader(cm.ExecuteReader());
        try
        {
          while(dr.Read())
          {
            ResourceInfo info = new ResourceInfo();
            info.ID = dr.GetString(0);
            info.Name = dr.GetString(1) + ", " + dr.GetString(2);
            List.Add(info);
          }
        }
        finally
        {
          dr.Close();
        }
      }
      finally
      {
        cn.Close();
      }
    }

    #endregion

  }
}
