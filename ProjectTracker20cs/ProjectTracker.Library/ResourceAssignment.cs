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
    private byte[] _timestamp = new byte[8];

    public Guid ProjectId
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

    #region Factory Methods

    internal static ResourceAssignment NewResourceAssignment(
      Project project, int role)
    {
      return new ResourceAssignment(project, role);
    }

    internal static ResourceAssignment NewResourceAssignment(
      Guid projectId, int role)
    {
      return new ResourceAssignment(Project.GetProject(projectId), role);
    }

    internal static ResourceAssignment NewResourceAssignment(
      Guid projectId)
    {
      return new ResourceAssignment(Project.GetProject(projectId), RoleList.DefaultRole());
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
      _projectId = dr.GetGuid("ProjectId");
      _projectName = dr.GetString("Name");
      _assigned = dr.GetSmartDate("Assigned");
      _role = dr.GetInt32("Role");
      dr.GetBytes("LastChanged", 0, _timestamp, 0, 8);
      MarkOld();
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

        using (SqlDataReader dr = cm.ExecuteReader())
        {
          dr.Read();
          dr.GetBytes(0, 0, _timestamp, 0, 8);
        }

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
        cm.Parameters.AddWithValue("@lastChanged", _timestamp);

        using (SqlDataReader dr = cm.ExecuteReader())
        {
          dr.Read();
          dr.GetBytes(0, 0, _timestamp, 0, 8);
        }

        MarkOld();
      }
    }

    private void LoadParameters(SqlCommand cm, Resource resource)
    {
      cm.Parameters.AddWithValue("@projectId", _projectId);
      cm.Parameters.AddWithValue("@resourceId", resource.Id);
      cm.Parameters.AddWithValue("@assigned", _assigned.DBValue);
      cm.Parameters.AddWithValue("@role", _role);
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
        cm.Parameters.AddWithValue("@projectId", _projectId);
        cm.Parameters.AddWithValue("@resourceId", resource.Id);

        cm.ExecuteNonQuery();

        MarkNew();
      }
    }

    #endregion

  }
}
