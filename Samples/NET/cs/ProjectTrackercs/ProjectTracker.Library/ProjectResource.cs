using Csla;
using System;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResource : BusinessBase<ProjectResource>, IHoldRoles
  {
    #region  Business Methods

    private static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    private static PropertyInfo<int> ResourceIdProperty = 
      RegisterProperty<int>(p=>p.ResourceId, "Resource id");
    public int ResourceId
    {
      get { return GetProperty(ResourceIdProperty); }
    }

    private static PropertyInfo<string> FirstNameProperty =
      RegisterProperty<string>(p=>p.FirstName, "First name");
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
    }

    private static PropertyInfo<string> LastNameProperty =
      RegisterProperty<string>(p=>p.LastName, "Last name");
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
    }

    public string FullName
    {
      get { return string.Format("{0}, {1}", LastName, FirstName); }
    }

    private static PropertyInfo<SmartDate> AssignedProperty =
      RegisterProperty<SmartDate>(p=>p.Assigned, "Date assigned");
    public string Assigned
    {
      get { return GetPropertyConvert<SmartDate, string>(AssignedProperty); }
    }

    private static PropertyInfo<int> RoleProperty = 
      RegisterProperty<int>(p=>p.Role, "Role assigned");
    public int Role
    {
      get { return GetProperty(RoleProperty); }
      set { SetProperty(RoleProperty, value); }
    }

    //public static readonly MethodInfo GetResourceMethod = RegisterMethod(typeof(ProjectResource), "GetResource");
    //public Resource GetResource()
    //{
    //  CanExecuteMethod(GetResourceMethod, true);
    //  return Resource.GetResource(GetProperty(ResourceIdProperty));
    //}

    public override string ToString()
    {
      return ResourceId.ToString();
    }

    #endregion

    #region  Business Rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Assignment.ValidRole { PrimaryProperty = RoleProperty });

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, "ProjectManager"));
      //BusinessRules.AddRule(new Csla.Rules.CommonRules.IsNotInRole(Csla.Rules.AuthorizationActions.ExecuteMethod, GetResourceMethod, "Guest"));
    }

    #endregion

    #region NewProjectResource

    internal static ProjectResource NewProjectResource(int resourceId)
    {
      return DataPortal.CreateChild<ProjectResource>(
        resourceId, RoleList.DefaultRole());
    }

#if SILVERLIGHT
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void Child_Create()
#else
    protected override void Child_Create()
#endif
    {
      LoadProperty(AssignedProperty, new SmartDate(System.DateTime.Today));
    }

    #endregion

#if !SILVERLIGHT
    #region  Factory Methods

    internal static ProjectResource GetResource(
      ProjectTracker.DalLinq.Assignment data)
    {
      return DataPortal.FetchChild<ProjectResource>(data);
    }

    private ProjectResource()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

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
      TimeStamp = data.LastChanged.ToArray();
    }

    private void Child_Insert(Project project)
    {
      TimeStamp = Assignment.AddAssignment(
        project.Id, 
        ReadProperty(ResourceIdProperty), 
        ReadProperty(AssignedProperty), 
        ReadProperty(RoleProperty));
    }

    private void Child_Update(Project project)
    {
      TimeStamp = Assignment.UpdateAssignment(
        project.Id, 
        ReadProperty(ResourceIdProperty), 
        ReadProperty(AssignedProperty), 
        ReadProperty(RoleProperty), 
        TimeStamp);
    }

    private void Child_DeleteSelf(Project project)
    {
      Assignment.RemoveAssignment(
        project.Id, 
        ReadProperty(ResourceIdProperty));
    }

    #endregion
#endif
  }
}