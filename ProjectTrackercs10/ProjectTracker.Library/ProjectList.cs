using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectList : ReadOnlyCollectionBase
	{
    #region Data Structure

    [Serializable()]
      public struct ProjectInfo
    {
      // this has private members, public properties because
      // ASP.NET can't databind to public members of a structure...
      private Guid _ID;
      private string _Name;

      public Guid ID
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


      public bool Equals(ProjectInfo info)
      {
        return _ID.Equals(info.ID);
      }
    }

    #endregion

    #region Business Properties and Methods

    public ProjectInfo this [int index]
    {
      get
      {
        return (ProjectInfo)List[index];
      }
    }

    #endregion

    #region Contains

    public bool Contains(ProjectInfo item)
    {
      foreach(ProjectInfo child in List)
        if(child.Equals(item))
          return true;
      return false;
    }

    #endregion

    #region Shared Methods

    public static ProjectList GetProjectList()
    {
      return (ProjectList)DataPortal.Fetch(new Criteria());
    }

    #endregion

    #region Criteria

    [Serializable()]
    public class Criteria
    {
      // no criteria - we retrieve all projects
    }

    #endregion

    #region Constructors

    private ProjectList()
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
        cm.CommandText = "getProjects";

        SafeDataReader dr = new SafeDataReader(cm.ExecuteReader());
        try
        {
          while(dr.Read())
          {
            ProjectInfo info = new ProjectInfo();
            info.ID = dr.GetGuid(0);
            info.Name = dr.GetString(1);
            base.InnerList.Add(info);
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
