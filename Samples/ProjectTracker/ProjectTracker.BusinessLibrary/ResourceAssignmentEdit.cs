using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ResourceAssignmentEdit : BusinessBase<ResourceAssignmentEdit>
  {
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial byte[] TimeStamp { get; set; }

    [Display(Name = "Project id")]
    public partial int ProjectId { get; private set; }

    [Display(Name = "Project name")]
    public partial string ProjectName { get; private set; }

    [Display(Name = "Date assigned")]
    public partial SmartDate Assigned { get; private set; }

    public string AssignedText
    {
      get { return Assigned.Text; }
    }

    [Display(Name = "Role assigned")]
    public partial int Role { get; set; }

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
        Assigned = DateTime.Today;
        var project = dal.Fetch(projectId) ?? throw new DataNotFoundException("Project");
        ProjectName = project.Name ?? string.Empty;
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
        Assigned = data.Assigned;
        TimeStamp = data.LastChanged ?? [];
        var project = dal.Fetch(data.ProjectId) ?? throw new DataNotFoundException("Project");
        ProjectName = project.Name ?? string.Empty;
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
          Assigned = this.Assigned,
          RoleId = this.Role
        };
        dal.Insert(item);
        TimeStamp = item.LastChanged ?? [];
      }
    }

    [UpdateChild]
    private void Update(ResourceEdit resource, [Inject] IAssignmentDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = dal.Fetch(ProjectId, resource.Id) ?? throw new DataNotFoundException("Assignment");
        item.Assigned = this.Assigned;
        item.RoleId = Role;
        item.LastChanged = TimeStamp;
        dal.Update(item);
        TimeStamp = item.LastChanged ?? [];
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