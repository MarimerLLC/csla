using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignment : BusinessBase<ResourceAssignment>
  {

    #region Business Methods

    private Guid _projectId = Guid.Empty;
    private string _projectName = string.Empty;
    private SmartDate _assigned = new SmartDate(DateTime.Today);
    private int _role;

    public Guid ProjectID
    {
      get
      {
        CanReadProperty(true);
        return _projectId;
      }
    }

    public string ProjectName
    {
      get
      {
        CanReadProperty(true);
        return _projectName;
      }
    }

    public string Assigned
    {
      get
      {
        CanReadProperty(true);
        return _assigned.Text;
      }
    }

    public int Role
    {
      get
      {
        CanReadProperty(true);
        return _role;
      }
      set
      {
        CanWriteProperty(true);
        if (_role.Equals(value))
        {
          _role = value;
          PropertyHasChanged();
        }
      }
    }

    public Project GetProject()
    {
      return Project.GetProject(_projectId);
    }

    protected override object GetIdValue()
    {
      return _projectId;
    }

    #endregion

    #region Constructors

    private ResourceAssignment()
    {
      MarkAsChild();
    }

    #endregion

    #region Shared Methods

    internal static ResourceAssignment NewResourceAssignment(
      Project project, int role)
    {
      return new ResourceAssignment(project, role);
    }

    internal static ResourceAssignment NewResourceAssignment(
      Guid projectID, int role)
    {
      return new ResourceAssignment(Project.GetProject(projectID), role);
    }

    internal static ResourceAssignment NewResourceAssignment(
      Guid projectID)
    {
      return new ResourceAssignment(Project.GetProject(projectID), RoleList.DefaultRole());
    }

    internal static ResourceAssignment GetResourceAssignment(
      SafeDataReader dr)
    {
      return new ResourceAssignment(dr);
    }

    #endregion

    #region Data Access

    private ResourceAssignment(Project project, int role)
    {
      MarkAsChild();
      _projectId = project.Id;
      _projectName = project.Name;
      _assigned.Date = DateTime.Now;
      _role = role;
    }

    private ResourceAssignment(SafeDataReader dr)
    {
      MarkAsChild();
      _projectId = dr.GetGuid(0);
      _projectName = dr.GetString(1);
      _assigned.Date = dr.GetDateTime(2);
      _role = dr.GetInt32(3);
    }

    internal void Insert(SqlTransaction tr, Resource resource)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      using (SqlCommand cm = new SqlCommand())
      {
        cm.Connection = tr.Connection;
        cm.Transaction = tr;
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "addAssignment";
        LoadParameters(cm, resource);

        cm.ExecuteNonQuery();

        MarkOld();
      }
    }

    internal void Update(SqlTransaction tr, Resource resource)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      using (SqlCommand cm = new SqlCommand())
      {
        cm.Connection = tr.Connection;
        cm.Transaction = tr;
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "updateAssignment";
        LoadParameters(cm, resource);

        cm.ExecuteNonQuery();

        MarkOld();
      }
    }

    private void LoadParameters(SqlCommand cm, Resource resource)
    {
      cm.Parameters.AddWithValue("@ProjectID", _projectId);
      cm.Parameters.AddWithValue("@ResourceID", resource.Id);
      cm.Parameters.AddWithValue("@Assigned", _assigned.DBValue);
      cm.Parameters.AddWithValue("@Role", _role);
    }

    internal void DeleteSelf(SqlTransaction tr, Resource resource)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      // if we're new then don't update the database
      if (!this.IsNew) return;

      using (SqlCommand cm = new SqlCommand())
      {
        cm.Connection = tr.Connection;
        cm.Transaction = tr;
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "deleteAssignment";
        cm.Parameters.AddWithValue("@ProjectID", _projectId);
        cm.Parameters.AddWithValue("@ResourceID", resource.Id);

        cm.ExecuteNonQuery();

        MarkNew();
      }
    }

    #endregion

  }
}
