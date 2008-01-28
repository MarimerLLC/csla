using Csla;
using System;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResource : BusinessBase<ProjectResource>, IHoldRoles
  {

    #region  Business Methods

    private byte[] _timestamp = new byte[8];

    private static PropertyInfo<int> ResourceIdProperty = RegisterProperty<int>(typeof(ProjectResource), new PropertyInfo<int>("ResourceId", "Resource id"));
    public int ResourceId
    {
      get
      {
        return GetProperty<int>(ResourceIdProperty);
      }
    }

    private static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(typeof(ProjectResource), new PropertyInfo<string>("FirstName", "First name"));
    public string FirstName
    {
      get
      {
        return GetProperty<string>(FirstNameProperty);
      }
    }

    private static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(typeof(ProjectResource), new PropertyInfo<string>("LastName", "Last name"));
    public string LastName
    {
      get
      {
        return GetProperty<string>(LastNameProperty);
      }
    }

    public string FullName
    {
      get
      {
        return LastName + ", " + FirstName;
      }
    }

    private static PropertyInfo<SmartDate> AssignedProperty = RegisterProperty<SmartDate>(typeof(ProjectResource), new PropertyInfo<SmartDate>("Assigned", "Date assigned"));
    public string Assigned
    {
      get
      {
        return GetProperty<SmartDate, string>(AssignedProperty);
      }
    }

    private static PropertyInfo<int> RoleProperty = RegisterProperty<int>(typeof(ProjectResource), new PropertyInfo<int>("Role", "Role assigned"));
    public int Role
    {
      get
      {
        return GetProperty<int>(RoleProperty);
      }
      set
      {
        SetProperty<int>(RoleProperty, value);
      }
    }

    public Resource GetResource()
    {
      CanExecuteMethod("GetResource", true);
      return Resource.GetResource(GetProperty<int>(ResourceIdProperty));
    }

    public override string ToString()
    {
      return ResourceId.ToString();
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

    internal static ProjectResource NewProjectResource(int resourceId)
    {
      return DataPortal.CreateChild<ProjectResource>(resourceId, RoleList.DefaultRole());
    }

    internal static ProjectResource GetResource(ProjectTracker.DalLinq.Assignment data)
    {
      return DataPortal.FetchChild<ProjectResource>(data);
    }

    private ProjectResource()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void Child_Create()
    {
      LoadProperty<SmartDate>(AssignedProperty, new SmartDate(System.DateTime.Today));
    }

    private void Child_Create(int resourceId, int role)
    {
      var res = Resource.GetResource(resourceId);
      LoadProperty<int>(ResourceIdProperty, res.Id);
      LoadProperty<string>(LastNameProperty, res.LastName);
      LoadProperty<string>(FirstNameProperty, res.FirstName);
      LoadProperty<SmartDate>(AssignedProperty, Assignment.GetDefaultAssignedDate());
      LoadProperty<int>(RoleProperty, role);
    }

    private void Child_Fetch(ProjectTracker.DalLinq.Assignment data)
    {
      LoadProperty<int>(ResourceIdProperty, data.ResourceId);
      LoadProperty<string>(LastNameProperty, data.Resource.LastName);
      LoadProperty<string>(FirstNameProperty, data.Resource.FirstName);
      LoadProperty<SmartDate>(AssignedProperty, data.Assigned);
      LoadProperty<int>(RoleProperty, data.Role);
      _timestamp = data.LastChanged.ToArray();
    }

    private void Child_Insert(Project project)
    {
      _timestamp = Assignment.AddAssignment(
        project.Id, 
        ReadProperty<int>(ResourceIdProperty), 
        ReadProperty<SmartDate>(AssignedProperty), 
        ReadProperty<int>(RoleProperty));
    }

    private void Child_Update(Project project)
    {
      _timestamp = Assignment.UpdateAssignment(
        project.Id, 
        ReadProperty<int>(ResourceIdProperty), 
        ReadProperty<SmartDate>(AssignedProperty), 
        ReadProperty<int>(RoleProperty), 
        _timestamp);
    }

    private void Child_DeleteSelf(Project project)
    {
      Assignment.RemoveAssignment(
        project.Id, 
        ReadProperty<int>(ResourceIdProperty));
    }

    #endregion

  }
}