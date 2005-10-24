using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  public class ProjectList : ReadOnlyListBase<ProjectList.ProjectInfo>
  {
    #region ProjectInfo Class

    [Serializable()]
    public class ProjectInfo
    {
      private Guid _id;
      private string _name;

      public Guid Id
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


      public override bool Equals(object obj)
      {
        if (obj is ProjectInfo)
          return _id.Equals(((ProjectInfo)obj).Id);
        else
          return false;
      }

      public override int GetHashCode()
      {
        return _id.GetHashCode();
      }
    }

    #endregion

    #region Factory Methods

    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>(new Criteria());
    }

    #endregion

    #region Criteria

    [Serializable()]
    private class Criteria
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

    private string DbConn
    {
      get
      {
        return System.Configuration.ConfigurationManager.ConnectionStrings["PTracker"].ConnectionString;
      }
    }

    protected override void DataPortal_Fetch(object criteria)
    {
      using (SqlConnection cn = new SqlConnection(DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getProjects";

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            IsReadOnly = false;
            while (dr.Read())
            {
              ProjectInfo info = new ProjectInfo();
              info.Id = dr.GetGuid(0);
              info.Name = dr.GetString(1);
              this.Add(info);
            }
            IsReadOnly = true;
          }
        }
      }
    }

    #endregion

  }
}
