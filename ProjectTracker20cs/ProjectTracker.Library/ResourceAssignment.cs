using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignment : BusinessBase<ResourceAssignment>, IHoldRoles
  {
    #region Business Methods

    private Guid _projectId = Guid.Empty;
    private string _projectName = string.Empty;
    private SmartDate _assigned = new SmartDate(DateTime.Today);
    private int _role;
    private byte[] _timestamp = new byte[8];

    public Guid ProjectId
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _projectId;
      }
    }

    public string ProjectName
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _projectName;
      }
    }

    public string Assigned
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _assigned.Text;
      }
    }

    public int Role
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _role;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_role.Equals(value))
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

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(
        new Csla.Validation.RuleHandler(
          Assignment.ValidRole), "Role");
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(
        "Role", "ProjectManager");
    }

    #endregion

    #region Factory Methods

    internal static ResourceAssignment NewResourceAssignment(
      Guid projectId)
    {
      return new ResourceAssignment(
        Project.GetProject(projectId), RoleList.DefaultRole());
    }

    internal static ResourceAssignment GetResourceAssignment(
      SafeDataReader dr)
    {
      return new ResourceAssignment(dr);
    }

    private ResourceAssignment()
    { MarkAsChild(); }

    #endregion

    #region Data Access

    /// <summary>
    /// Called to when a new object is created.
    /// </summary>
    private ResourceAssignment(Project project, int role)
    {
      MarkAsChild();
      _projectId = project.Id;
      _projectName = project.Name;
      _assigned.Date = Assignment.GetDefaultAssignedDate();
      _role = role;
    }

    /// <summary>
    /// Called when loading data from the database.
    /// </summary>
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

    internal void Insert(SqlConnection cn, Resource resource)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      _timestamp = Assignment.AddAssignment(
        cn, _projectId, resource.Id, _assigned, _role);
      MarkOld();
    }

    internal void Update(SqlConnection cn, Resource resource)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      _timestamp = Assignment.UpdateAssignment(
        cn, _projectId, resource.Id, _assigned, _role, _timestamp);
      MarkOld();
    }

    internal void DeleteSelf(SqlConnection cn, Resource resource)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      // if we're new then don't update the database
      if (this.IsNew) return;

      Assignment.RemoveAssignment(cn, _projectId, resource.Id);
      MarkNew();
    }

    #endregion

  }
}
