using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResource : Assignment
  {
    string _ResourceID = string.Empty;
    string _LastName = string.Empty;
    string _FirstName = string.Empty;

    #region Business Properties and Methods

    public string ResourceID
    {
      get
      {
        return _ResourceID;
      }
    }

    public string LastName
    {
      get
      {
        return _LastName;
      }
    }

    public string FirstName
    {
      get
      {
        return _FirstName;
      }
    }

    public Resource GetResource()
    {
      return Resource.GetResource(_ResourceID);
    }

    #endregion

    #region System.Object Overrides

    public override string ToString()
    {
      return _LastName + ", " + _FirstName;
    }

    public new static bool Equals(object objA, object objB)
    {
      if(objA is ProjectResource && objB is ProjectResource)
        return ((ProjectResource)objA).Equals((ProjectResource)objB);
      else
        return false;
    }

    public override bool Equals(object projectResource)
    {
      if(projectResource is ProjectResource)
        return Equals((ProjectResource)projectResource);

      else
        return false;
    }

    public bool Equals(ProjectResource assignment)
    {
      return _ResourceID == assignment.ResourceID;
    }

    public override int GetHashCode()
    {
      return _ResourceID.GetHashCode();
    }

    #endregion

    #region Static Methods

    internal static ProjectResource NewProjectResource(
      Resource resource, string role)
    {
      return new ProjectResource(resource, role);
    }

    internal static ProjectResource NewProjectResource(
      string resourceID, string role)
    {
      return new ProjectResource(Resource.GetResource(resourceID), role);
    }

    internal static ProjectResource NewProjectResource(
      string resourceID)
    {
      return new ProjectResource(Resource.GetResource(resourceID), DefaultRole);
    }

    internal static ProjectResource GetProjectResource(SafeDataReader dr) 
    {
      ProjectResource child = new ProjectResource();
      child.Fetch(dr);
      return child;
    }

    #endregion

    #region Constructors

    private ProjectResource(Resource resource, string role)
    {
      _ResourceID = resource.ID;
      _LastName = resource.LastName;
      _FirstName = resource.FirstName;
      _Assigned.Date = DateTime.Now;
      _Role = Convert.ToInt32(Roles.Key(Role));
    }

    private ProjectResource()
    {
      // prevent direct creation of this object
    }

    #endregion

    #region Data Access

    private void Fetch(SafeDataReader dr)
    {
      _ResourceID = dr.GetString(0);
      _LastName = dr.GetString(1);
      _FirstName = dr.GetString(2);
      _Assigned = dr.GetSmartDate(3);
      _Role = dr.GetInt32(4);
      MarkOld();
    }

    internal void Update(Project project)
    {
      // if we're not dirty then don't update the database
      if(!this.IsDirty)
        return;

      // do the update 
      SqlConnection cn = new SqlConnection(DB("PTracker"));
      cn.Open();

      try
      {
        SqlCommand cm = new SqlCommand();
        cm.Connection = cn;
        cm.CommandType = CommandType.StoredProcedure;
        if(this.IsDeleted)
        {
          if(!this.IsNew)
          {
            // we're not new, so delete
            cm.CommandText = "deleteAssignment";
            cm.Parameters.Add("@ProjectID", project.ID);
            cm.Parameters.Add("@ResourceID", _ResourceID);

            cm.ExecuteNonQuery();

            MarkNew();
          }
        }
        else
          // we are either adding or updating
          if(this.IsNew)
        {
          // we're new, so insert
          cm.CommandText = "addAssignment";
        }
        else
        {
          // we're not new, so update
          cm.CommandText = "updateAssignment";
        }
        cm.Parameters.Add("@ProjectID", project.ID);
        cm.Parameters.Add("@ResourceID", _ResourceID);
        cm.Parameters.Add("@Assigned", _Assigned.DBValue);
        cm.Parameters.Add("@Role", _Role);

        cm.ExecuteNonQuery();

        MarkOld();
      }
      finally
      {
        cn.Close();
      }
    }

    #endregion


  }
}
