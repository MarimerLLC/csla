using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  public class ProjectList : ReadOnlyListBase<ProjectList, ProjectList.ProjectInfo>
  {

    #region ProjectInfo Class

    [Serializable()]
    public class ProjectInfo
    {
      private Guid _id;
      private string _name;

      public Guid Id
      {
        get { return _id; }
        internal set { _id = value; }
      }

      public string Name
      {
        get { return _name; }
        internal set { _name = value; }
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

    #region Criteria

    [Serializable()]
    private class Criteria
    {
      // no criteria - retrieve all projects
    }

    [Serializable()]
    private class FilteredCriteria
    {
      private string _name;
      public string Name
      {
        get { return _name; }
      }

      public FilteredCriteria(string name)
      {
        _name = name;
      }
    }

    #endregion

    #region Constructors

    private ProjectList()
    {
      // prevent direct creation
    }

    #endregion

    #region Factory Methods

    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>(new Criteria());
    }

    public static ProjectList GetProjectList(string name)
    {
      return DataPortal.Fetch<ProjectList>(new FilteredCriteria(name));
    }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(Criteria criteria)
    {
      // fetch with no filter
      Fetch("");
    }

    private void DataPortal_Fetch(FilteredCriteria criteria)
    {
      Fetch(criteria.Name);
    }

    private void Fetch(string nameFilter)
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
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
              // apply filter if necessary
              if ((nameFilter.Length == 0) || (info.Name.IndexOf(nameFilter) == 0))
                this.Add(info);
            }
            IsReadOnly = true;
            dr.Close();
          }
        }
        cn.Close();
      }
    }

    #endregion

  }
}
