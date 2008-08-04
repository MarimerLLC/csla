using Csla;
using System;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignment : BusinessBase<ResourceAssignment>, IHoldRoles
  {
    #region  Business Methods

    private byte[] _timestamp = new byte[8];

    private static PropertyInfo<Guid> ProjectIdProperty = RegisterProperty(new PropertyInfo<Guid>("ProjectId", "Project id", Guid.Empty));
    private Guid _projectId = ProjectIdProperty.DefaultValue;
    public Guid ProjectId
    {
      get
      {
        return GetProperty<Guid>(ProjectIdProperty, _projectId);
      }
    }

    private static PropertyInfo<string> ProjectNameProperty = RegisterProperty(new PropertyInfo<string>("ProjectName"));
    private string _projectName = ProjectNameProperty.DefaultValue;
    public string ProjectName
    {
      get
      {
        return GetProperty<string>(ProjectNameProperty, _projectName);
      }
    }

    private static PropertyInfo<SmartDate> AssignedProperty = RegisterProperty(new PropertyInfo<SmartDate>("Assigned"));
    private SmartDate _assigned = new SmartDate(System.DateTime.Today);
    public string Assigned
    {
      get
      {
        return GetProperty<SmartDate, string>(AssignedProperty, _assigned);
      }
    }

    private static PropertyInfo<int> RoleProperty = RegisterProperty(new PropertyInfo<int>("Role"));
    private int _role = RoleProperty.DefaultValue;
    public int Role
    {
      get
      {
        return GetProperty<int>(RoleProperty, _role);
      }
      set
      {
        SetProperty<int>(RoleProperty, ref _role, value);
      }
    }

    public Project GetProject()
    {
      CanExecuteMethod("GetProject", true);
      return Project.GetProject(_projectId);
    }

    public override string ToString()
    {
      return _projectId.ToString();
    }

    #endregion

    #region  Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Assignment.ValidRole, RoleProperty);
    }

    #endregion

    #region  Authorization Rules

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(RoleProperty, "ProjectManager");
    }

    #endregion

    #region  Factory Methods

    internal static ResourceAssignment NewResourceAssignment(Guid projectId)
    {
      return DataPortal.CreateChild<ResourceAssignment>(projectId, RoleList.DefaultRole());
    }

    internal static ResourceAssignment GetResourceAssignment(ProjectTracker.DalLinq.Assignment data)
    {
      return DataPortal.FetchChild<ResourceAssignment>(data);
    }

    private ResourceAssignment()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void Child_Create(Guid projectId, int role)
    {
      var proj = Project.GetProject(projectId);
      _projectId = proj.Id;
      _projectName = proj.Name;
      _assigned.Date = Assignment.GetDefaultAssignedDate();
      _role = role;
    }

    private void Child_Fetch(ProjectTracker.DalLinq.Assignment data)
    {
      _projectId = data.ProjectId;
      _projectName = data.Project.Name;
      _assigned = data.Assigned;
      _role = data.Role;
      _timestamp = data.LastChanged.ToArray();
    }

    private void Child_Insert(Resource resource)
    {
      _timestamp = Assignment.AddAssignment(_projectId, resource.Id, _assigned, _role);
    }

    private void Child_Update(Resource resource)
    {
      _timestamp = Assignment.UpdateAssignment(_projectId, resource.Id, _assigned, _role, _timestamp);
    }

    private void Child_DeleteSelf(Resource resource)
    {
      Assignment.RemoveAssignment(_projectId, resource.Id);
    }

    #endregion

  }
}