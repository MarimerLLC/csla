using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignment : Assignment
	{
    Guid _ProjectID = Guid.Empty;
    string _ProjectName = string.Empty;

    #region Business Properties and Methods

    public Guid ProjectID
    {
      get
      {
        return _ProjectID;
      }
    }

    public string ProjectName
    {
      get
      {
        return _ProjectName;
      }
    }

    public Project GetProject()
    {
      return Project.GetProject(_ProjectID);
    }

    #endregion

    #region System.Object Overrides

    public override string ToString()
    {
      return _ProjectName;
    }

    public new static bool Equals(object objA, object objB)
    {
      if(objA is ResourceAssignment && objB is ResourceAssignment)
        return ((ResourceAssignment)objA).Equals((ResourceAssignment)objB);
      else
        return false;
    }

    public override bool Equals(object resourceAssignment)
    {
      if(resourceAssignment is ResourceAssignment)
        return Equals((ResourceAssignment)resourceAssignment);
      else
        return false;
    }

    public bool Equals(ResourceAssignment assignment)
    {
      return _ProjectID.Equals(assignment.ProjectID);
    }

    public override int GetHashCode()
    {
      return _ProjectID.GetHashCode();
    }

    #endregion

    #region Static Methods

    internal static ResourceAssignment NewResourceAssignment(Project project, string role) 
    {
        return new ResourceAssignment(project, role);
    }

    internal static ResourceAssignment NewResourceAssignment(Guid projectID, string role)
    {
      return new ResourceAssignment(Project.GetProject(projectID), role);
    }

    internal static ResourceAssignment NewResourceAssignment(Guid projectID)
    {
      return new ResourceAssignment(Project.GetProject(projectID), DefaultRole);
    }

    internal static ResourceAssignment GetResourceAssignment(SafeDataReader dr)
    {
      ResourceAssignment child = new ResourceAssignment();
      child.Fetch(dr);
      return child;
    }

    #endregion

    #region Constructors 

    private ResourceAssignment(Project project, string role)
    {
      _ProjectID = project.ID;
      _ProjectName = project.Name;
      _Assigned.Date = DateTime.Now;
      _Role = Convert.ToInt32(Roles.Key(role));
      MarkAsChild();
    }

    private ResourceAssignment()
    {
      // prevent direct creation of this object
    }

    #endregion

    #region Data Access

    private void Fetch(SafeDataReader dr)
    {
      _ProjectID = dr.GetGuid(0);
      _ProjectName = dr.GetString(1);
      _Assigned.Date = dr.GetDateTime(2);
      _Role = dr.GetInt32(3);
      MarkOld();
    }

    internal void Update(SqlTransaction tr, Resource resource)
    {
      // if we're not dirty then don't update the database
      if(!this.IsDirty)
        return;

      // do the update 
      SqlCommand cm = new SqlCommand();
      cm.Connection = tr.Connection;
      cm.Transaction = tr;
      cm.CommandType = CommandType.StoredProcedure;

      if(this.IsDeleted)
      {
        if(!this.IsNew)
        {
          // we're not new, so delete
          cm.CommandText = "deleteAssignment";
          cm.Parameters.Add("@ProjectID", _ProjectID);
          cm.Parameters.Add("@ResourceID", resource.ID);

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
        cm.Parameters.Add("@ProjectID", _ProjectID);
        cm.Parameters.Add("@ResourceID", resource.ID);
        cm.Parameters.Add("@Assigned", _Assigned.DBValue);
        cm.Parameters.Add("@Role", _Role);
        cm.ExecuteNonQuery();

        MarkOld();
      }
    }

    #endregion

	}
}
