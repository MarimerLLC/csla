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
    string _resourceID = string.Empty;
    string _lastName = string.Empty;
    string _firstName = string.Empty;

    #region Business Properties and Methods

    public string ResourceID
    {
      get
      {
        return _resourceID;
      }
    }

    public string LastName
    {
      get
      {
        return _lastName;
      }
    }

    public string FirstName
    {
      get
      {
        return _firstName;
      }
    }

    public Resource GetResource()
    {
      return Resource.GetResource(_resourceID);
    }

    #endregion

    #region System.Object Overrides

    public override string ToString()
    {
      return _lastName + ", " + _firstName;
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
      return _resourceID == assignment.ResourceID;
    }

    public override int GetHashCode()
    {
      return _resourceID.GetHashCode();
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
      _resourceID = resource.ID;
      _lastName = resource.LastName;
      _firstName = resource.FirstName;
      _assigned.Date = DateTime.Now;
      _role = Convert.ToInt32(Roles.Key(role));
    }

    private ProjectResource()
    {
      // prevent direct creation of this object
    }

    #endregion

    #region Data Access

    private void Fetch(SafeDataReader dr)
    {
      _resourceID = dr.GetString(0);
      _lastName = dr.GetString(1);
      _firstName = dr.GetString(2);
      _assigned = dr.GetSmartDate(3);
      _role = dr.GetInt32(4);
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
            cm.Parameters.Add("@ResourceID", _resourceID);

            cm.ExecuteNonQuery();

            MarkNew();
          }
        }
        else
        {
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
          cm.Parameters.Add("@ResourceID", _resourceID);
          cm.Parameters.Add("@Assigned", _assigned.DBValue);
          cm.Parameters.Add("@Role", _role);

          cm.ExecuteNonQuery();

          MarkOld();
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
