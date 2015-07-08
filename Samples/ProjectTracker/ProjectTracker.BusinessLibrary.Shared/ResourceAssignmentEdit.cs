using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;
using System.ComponentModel;

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
        if (RoleList.GetList().ContainsKey(Role))
          result = RoleList.GetList().GetItemByKey(Role).Value;
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

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, "ProjectManager"));
    }

#if !NETFX_CORE
    private void Child_Create(int projectId)
    {
      using (BypassPropertyChecks)
      {
        ProjectId = projectId;
        Role = RoleList.DefaultRole();
        LoadProperty(AssignedProperty, DateTime.Today);
        using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
        {
          var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
          var project = dal.Fetch(projectId);
          ProjectName = project.Name;
        }
      }
      base.Child_Create();
    }

    private void Child_Fetch(ProjectTracker.Dal.AssignmentDto data)
    {
      using (BypassPropertyChecks)
      {
        ProjectId = data.ProjectId;
        Role = data.RoleId;
        LoadProperty(AssignedProperty, data.Assigned);
        TimeStamp = data.LastChanged;
        using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
        {
          var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
          var project = dal.Fetch(data.ProjectId);
          ProjectName = project.Name;
        }
      }
    }

    private void Child_Insert(ResourceEdit resource)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
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
    }

    private void Child_Update(ResourceEdit resource)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
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
    }

    private void Child_DeleteSelf(ResourceEdit resource)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IAssignmentDal>();
        using (BypassPropertyChecks)
        {
          dal.Delete(ProjectId, resource.Id);
        }
      }
    }
#endif
  }
}