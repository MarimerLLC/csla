using Csla;
using System;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignment : BusinessBase<ResourceAssignment>, IHoldRoles
  {
    #region  Business Methods

    private static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    private static PropertyInfo<Guid> ProjectIdProperty = 
      RegisterProperty<Guid>(r=>r.ProjectId, "Project id", Guid.Empty);
    public Guid ProjectId
    {
      get { return GetProperty(ProjectIdProperty); }
      private set { SetProperty(ProjectIdProperty, value); }
    }

    private static PropertyInfo<string> ProjectNameProperty = 
      RegisterProperty<string>(r=>r.ProjectName);
    public string ProjectName
    {
      get { return GetProperty(ProjectNameProperty); }
      private set { SetProperty(ProjectNameProperty, value); }
    }

    private static PropertyInfo<SmartDate> AssignedProperty = 
      RegisterProperty<SmartDate>(r=>r.Assigned);
    public string Assigned
    {
      get { return GetPropertyConvert<SmartDate, string>(AssignedProperty); }
    }

    private static PropertyInfo<int> RoleProperty = RegisterProperty<int>(r=>r.Role);
    public int Role
    {
      get { return GetProperty(RoleProperty); }
      set { SetProperty(RoleProperty, value); }
    }

    public Project GetProject()
    {
      CanExecuteMethod("GetProject", true);
      return Project.GetProject(ProjectId);
    }

    public override string ToString()
    {
      return ProjectId.ToString();
    }

    #endregion

    #region  Business Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule<ResourceAssignment>(Assignment.ValidRole, RoleProperty);
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
      using (BypassPropertyChecks)
      {
        ProjectId = proj.Id;
        ProjectName = proj.Name;
        LoadPropertyConvert<SmartDate, DateTime>(AssignedProperty, Assignment.GetDefaultAssignedDate());
        Role = role;
      }
    }

    private void Child_Fetch(ProjectTracker.DalLinq.Assignment data)
    {
      using (BypassPropertyChecks)
      {
        ProjectId = data.ProjectId;
        ProjectName = data.Project.Name;
        LoadPropertyConvert<SmartDate, DateTime>(AssignedProperty, data.Assigned);
        Role = data.Role;
        TimeStamp = data.LastChanged.ToArray();
      }
    }

    private void Child_Insert(Resource resource)
    {
      using (BypassPropertyChecks)
      {
        TimeStamp = Assignment.AddAssignment(
          ProjectId, resource.Id, ReadProperty(AssignedProperty), Role);
      }
    }

    private void Child_Update(Resource resource)
    {
      using (BypassPropertyChecks)
      {
        TimeStamp = Assignment.UpdateAssignment(
          ProjectId, resource.Id, ReadProperty(AssignedProperty), Role, TimeStamp);
      }
    }

    private void Child_DeleteSelf(Resource resource)
    {
      using (BypassPropertyChecks)
      {
        Assignment.RemoveAssignment(ProjectId, resource.Id);
      }
    }

    #endregion

  }
}