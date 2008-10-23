using Csla;
using System;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResource : BusinessBase<ProjectResource>, IHoldRoles
  {
    #region  Business Methods

    private byte[] _timestamp = new byte[8];

    private static PropertyInfo<int> ResourceIdProperty = 
      RegisterProperty(new PropertyInfo<int>("ResourceId", "Resource id"));
    public int ResourceId
    {
      get
      {
        return GetProperty(ResourceIdProperty);
      }
    }

    private static PropertyInfo<string> FirstNameProperty = 
      RegisterProperty(new PropertyInfo<string>("FirstName", "First name"));
    public string FirstName
    {
      get
      {
        return GetProperty(FirstNameProperty);
      }
    }

    private static PropertyInfo<string> LastNameProperty = 
      RegisterProperty(new PropertyInfo<string>("LastName", "Last name"));
    public string LastName
    {
      get
      {
        return GetProperty(LastNameProperty);
      }
    }

    public string FullName
    {
      get
      {
        return LastName + ", " + FirstName;
      }
    }

    private static PropertyInfo<SmartDate> AssignedProperty = 
      RegisterProperty(new PropertyInfo<SmartDate>("Assigned", "Date assigned"));
    public string Assigned
    {
      get
      {
        return GetPropertyConvert<SmartDate, string>(AssignedProperty);
      }
    }

    private static PropertyInfo<int> RoleProperty = 
      RegisterProperty(new PropertyInfo<int>("Role", "Role assigned"));
    public int Role
    {
      get
      {
        return GetProperty(RoleProperty);
      }
      set
      {
        SetProperty(RoleProperty, value);
      }
    }

    public Resource GetResource()
    {
      CanExecuteMethod("GetResource", true);
      return Resource.GetResource(GetProperty(ResourceIdProperty));
    }

    public override string ToString()
    {
      return ResourceId.ToString();
    }

    #endregion

    #region  Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule<ProjectResource>(
        Assignment.ValidRole, RoleProperty);
    }

    #endregion

    #region  Authorization Rules

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(RoleProperty, "ProjectManager");
      AuthorizationRules.DenyExecute("GetResource", "Guest");
    }

    #endregion

    #region  Factory Methods

    internal static ProjectResource NewProjectResource(int resourceId)
    {
      return DataPortal.CreateChild<ProjectResource>(
        resourceId, RoleList.DefaultRole());
    }

    internal static ProjectResource GetResource(
      ProjectTracker.DalLinq.Assignment data)
    {
      return DataPortal.FetchChild<ProjectResource>(data);
    }

    private ProjectResource()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    protected override void Child_Create()
    {
      LoadProperty(AssignedProperty, new SmartDate(System.DateTime.Today));
    }

    private void Child_Create(int resourceId, int role)
    {
      var res = Resource.GetResource(resourceId);
      LoadProperty(ResourceIdProperty, res.Id);
      LoadProperty(LastNameProperty, res.LastName);
      LoadProperty(FirstNameProperty, res.FirstName);
      LoadProperty(AssignedProperty, Assignment.GetDefaultAssignedDate());
      LoadProperty(RoleProperty, role);
    }

    private void Child_Fetch(ProjectTracker.DalLinq.Assignment data)
    {
      LoadProperty(ResourceIdProperty, data.ResourceId);
      LoadProperty(LastNameProperty, data.Resource.LastName);
      LoadProperty(FirstNameProperty, data.Resource.FirstName);
      LoadProperty(AssignedProperty, data.Assigned);
      LoadProperty(RoleProperty, data.Role);
      _timestamp = data.LastChanged.ToArray();
    }

    private void Child_Insert(Project project)
    {
      _timestamp = Assignment.AddAssignment(
        project.Id, 
        ReadProperty(ResourceIdProperty), 
        ReadProperty(AssignedProperty), 
        ReadProperty(RoleProperty));
    }

    private void Child_Update(Project project)
    {
      _timestamp = Assignment.UpdateAssignment(
        project.Id, 
        ReadProperty(ResourceIdProperty), 
        ReadProperty(AssignedProperty), 
        ReadProperty(RoleProperty), 
        _timestamp);
    }

    private void Child_DeleteSelf(Project project)
    {
      Assignment.RemoveAssignment(
        project.Id, 
        ReadProperty(ResourceIdProperty));
    }

    #endregion

  }
}