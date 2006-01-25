using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectList : 
    ReadOnlyListBase<ProjectList, ProjectList.ProjectInfo>
  {
    #region ProjectInfo Class

    [Serializable()]
    public class ProjectInfo :
      ReadOnlyBase<ProjectInfo>
    {
      #region Business Methods

      private Guid _id;
      private string _name;

      public Guid Id
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

      public override string ToString()
      {
        return _name;
      }
      
      #endregion

      #region Constructors

      private ProjectInfo()
      { /* require use of factory methods */ }

      internal ProjectInfo(Guid id, string name)
      {
        _id = id;
        _name = name;
      }
      #endregion
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Return a list of all projects.
    /// </summary>
    public static ProjectList GetProjectList()
    {
      return DataPortal.Fetch<ProjectList>(new Criteria());
    }

    /// <summary>
    /// Return a list of projects filtered
    /// by project name.
    /// </summary>
    public static ProjectList GetProjectList(string name)
    {
      return DataPortal.Fetch<ProjectList>
        (new FilteredCriteria(name));
    }

    private ProjectList()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    { /* no criteria - retrieve all projects */ }

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
      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
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
              ProjectInfo info = new ProjectInfo(
                dr.GetGuid(0),
                dr.GetString(1));
              // apply filter if necessary
              if ((nameFilter.Length == 0) || (info.Name.IndexOf(nameFilter) == 0))
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
