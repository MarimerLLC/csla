using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignmentEdit : BusinessBase<ResourceAssignmentEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(c => c.ProjectId);
    [Display(Name = "Project id")]
    public int ProjectId
    {
      get { return GetProperty(ProjectIdProperty); }
      private set { SetProperty(ProjectIdProperty, value); }
    }

    public static readonly PropertyInfo<string> ProjectNameProperty = 
      RegisterProperty<string>(c => c.ProjectName);
    [Display(Name = "Project name")]
    public string ProjectName
    {
      get { return GetProperty(ProjectNameProperty); }
      private set { SetProperty(ProjectNameProperty, value); }
    }

    public static readonly PropertyInfo<SmartDate> AssignedProperty = 
      RegisterProperty<SmartDate>(c => c.Assigned);
    public string Assigned
    {
      get { return GetPropertyConvert<SmartDate, string>(AssignedProperty); }
    }

    public static readonly PropertyInfo<int> RoleProperty = RegisterProperty<int>(c => c.Role);
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
      return ProjectId.ToString();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new ValidRole(RoleProperty));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, Security.Roles.ProjectManager));
    }

    [CreateChild]
    private void Create(int projectId, [Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        ProjectId = projectId;
        RoleList.CacheList();
        Role = RoleList.DefaultRole();
        LoadProperty(AssignedProperty, DateTime.Today);
        var project = dal.Fetch(projectId);
        ProjectName = project.Name;
      }
      BusinessRules.CheckRules();
    }

    [FetchChild]
    private void Fetch(ProjectTracker.Dal.AssignmentDto data, [Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        ProjectId = data.ProjectId;
        Role = data.RoleId;
        LoadProperty(AssignedProperty, data.Assigned);
        TimeStamp = data.LastChanged;
        var project = dal.Fetch(data.ProjectId);
        ProjectName = project.Name;
      }
    }

    [InsertChild]
    private void Insert(ResourceEdit resource, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.AssignmentDto
        {
          ProjectId = this.ProjectId,
          ResourceId = resource.Id,
          Assigned = ReadProperty(AssignedProperty),
          RoleId = this.Role
        };
        dal.Insert(item);
        TimeStamp = item.LastChanged;
      }
    }

    [UpdateChild]
    private void Update(ResourceEdit resource, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = dal.Fetch(ProjectId, resource.Id);
        item.Assigned = ReadProperty(AssignedProperty);
        item.RoleId = Role;
        item.LastChanged = TimeStamp;
        dal.Update(item);
        TimeStamp = item.LastChanged;
      }
    }

    [DeleteSelfChild]
    private void DeleteSelf(ResourceEdit resource, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        dal.Delete(ProjectId, resource.Id);
      }
    }
  }
}