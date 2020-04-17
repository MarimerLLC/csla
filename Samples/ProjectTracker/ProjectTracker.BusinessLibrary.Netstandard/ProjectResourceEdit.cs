using Csla;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using Csla.Rules;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceEdit : BusinessBase<ProjectResourceEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> ResourceIdProperty = 
      RegisterProperty<int>(c => c.ResourceId);
    [Display(Name = "Resource id")]
    public int ResourceId
    {
      get { return GetProperty(ResourceIdProperty); }
      private set { LoadProperty(ResourceIdProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty =
      RegisterProperty<string>(c => c.FirstName);
    [Display(Name = "First name")]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      private set { LoadProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty =
      RegisterProperty<string>(c => c.LastName);
    [Display(Name = "Last name")]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      private set { LoadProperty(LastNameProperty, value); }
    }

    [Display(Name = "Full name")]
    public string FullName
    {
      get { return string.Format("{0}, {1}", LastName, FirstName); }
    }

    public static readonly PropertyInfo<DateTime> AssignedProperty =
      RegisterProperty<DateTime>(c => c.Assigned);
    [Display(Name = "Date assigned")]
    public DateTime Assigned
    {
      get { return GetProperty(AssignedProperty); }
      private set { LoadProperty(AssignedProperty, value); }
    }

    public static readonly PropertyInfo<int> RoleProperty = 
      RegisterProperty<int>(c => c.Role);
    [Display(Name = "Role assigned")]
    public int Role
    {
      get { return GetProperty(RoleProperty); }
      set { SetProperty(RoleProperty, value); }
    }

    [Display(Name = "Role")]
    public string RoleName
    {
      get 
      {
        var result = "none";
        if (RoleList.GetCachedList().ContainsKey(Role))
          result = RoleList.GetCachedList().GetItemByKey(Role).Value;
        return result;
      }
    }

    public override string ToString()
    {
      return ResourceId.ToString();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new ValidRole(RoleProperty));
      BusinessRules.AddRule(new NotifyRoleNameChanged(RoleProperty));
      BusinessRules.AddRule(
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, Security.Roles.ProjectManager));
    }

    private class NotifyRoleNameChanged : BusinessRule
    {
      public NotifyRoleNameChanged(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

#pragma warning disable CSLA0017 // Find Business Rules That Do Not Use Add() Methods on the Context
      protected override void Execute(IRuleContext context)
      {
        ((ProjectResourceEdit)context.Target).OnPropertyChanged(nameof(RoleName));
      }
#pragma warning restore CSLA0017 // Find Business Rules That Do Not Use Add() Methods on the Context
    }

    [CreateChild]
    private void Create(int resourceId, [Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        ResourceId = resourceId;
        RoleList.CacheList();
        Role = RoleList.DefaultRole();
        LoadProperty(AssignedProperty, DateTime.Today);
        var person = dal.Fetch(resourceId);
        FirstName = person.FirstName;
        LastName = person.LastName;
      }
      BusinessRules.CheckRules();
    }

    [FetchChild]
    private void Fetch(int projectId, int resourceId, [Inject] IAssignmentDal dal, [Inject] IResourceDal rdal)
    {
      var data = dal.Fetch(projectId, resourceId);
      Fetch(data, rdal);
    }

    [FetchChild]
    private void Fetch(ProjectTracker.Dal.AssignmentDto data, [Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        ResourceId = data.ResourceId;
        Role = data.RoleId;
        LoadProperty(AssignedProperty, data.Assigned);
        TimeStamp = data.LastChanged;
        var person = dal.Fetch(data.ResourceId);
        FirstName = person.FirstName;
        LastName = person.LastName;
      }
    }

    [InsertChild]
    private void Insert(ProjectEdit project, [Inject] IAssignmentDal dal)
    {
      Insert(project.Id, dal);
    }

    [InsertChild]
    private void Insert(int projectId, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.AssignmentDto
        {
          ProjectId = projectId,
          ResourceId = this.ResourceId,
          Assigned = ReadProperty(AssignedProperty),
          RoleId = this.Role
        };
        dal.Insert(item);
        TimeStamp = item.LastChanged;
      }
    }

    [UpdateChild]
    private void Update(ProjectEdit project, [Inject] IAssignmentDal dal)
    {
      Update(project.Id, dal);
    }

    [UpdateChild]
    private void Update(int projectId, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = dal.Fetch(projectId, ResourceId);
        item.Assigned = ReadProperty(AssignedProperty);
        item.RoleId = Role;
        item.LastChanged = TimeStamp;
        dal.Update(item);
        TimeStamp = item.LastChanged;
      }
    }

    [DeleteSelfChild]
    private void DeleteSelf(ProjectEdit project, [Inject] IAssignmentDal dal)
    {
      DeleteSelf(project.Id, dal);
    }

    [DeleteSelfChild]
    private void DeleteSelf(int projectId, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        dal.Delete(projectId, ResourceId);
      }
    }
  }
}