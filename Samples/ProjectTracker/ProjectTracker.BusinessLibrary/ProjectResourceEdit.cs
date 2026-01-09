using Csla;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using Csla.Rules;
using ProjectTracker.Dal;
using System.Linq;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectResourceEdit : BusinessBase<ProjectResourceEdit>
  {
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial byte[] TimeStamp { get; set; }

    [Display(Name = "Resource id")]
    public partial int ResourceId { get; private set; }

    [Display(Name = "First name")]
    public partial string FirstName { get; private set; }

    [Display(Name = "Last name")]
    public partial string LastName { get; private set; }

    [Display(Name = "Full name")]
    public string FullName
    {
      get { return string.Format("{0}, {1}", LastName, FirstName); }
    }

    [Display(Name = "Date assigned")]
    public partial DateTime Assigned { get; private set; }

    [Display(Name = "Role assigned")]
    public partial int Role { get; set; }

    public partial RoleList RoleList { get; private set; }

    public string RoleName
    {
      get
      {
        var list = RoleList;
        if (list is null)
          return string.Empty;
        var role = list.Where(r => r.Key == Role).FirstOrDefault();
        return role?.Value ?? string.Empty;
      }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, Security.Roles.ProjectManager));
    }

    [CreateChild]
    private void Create(int resourceId, [Inject]IResourceDal dal, [Inject]IDataPortal<RoleList> rolePortal)
    {
      using (BypassPropertyChecks)
      {
        ResourceId = resourceId;
        Assigned = DateTime.Today;
        var person = dal.Fetch(resourceId) ?? throw new DataNotFoundException("Resource");
        FirstName = person.FirstName ?? string.Empty;
        LastName = person.LastName ?? string.Empty;
        var roles = rolePortal.Fetch() ?? throw new InvalidOperationException("Unable to fetch roles.");
        RoleList = roles;
      }
      BusinessRules.CheckRules();
    }


    [FetchChild]
    private void Fetch(ProjectTracker.Dal.AssignmentDto data, [Inject] IResourceDal dal, [Inject] IDataPortal<RoleList> rolePortal)
    {
      using (BypassPropertyChecks)
      {
        ResourceId = data.ResourceId;
        Role = data.RoleId;
        Assigned = data.Assigned;
        TimeStamp = data.LastChanged ?? [];
        var person = dal.Fetch(data.ResourceId) ?? throw new DataNotFoundException("Resource");
        FirstName = person.FirstName ?? string.Empty;
        LastName = person.LastName ?? string.Empty;
        var roles = rolePortal.Fetch() ?? throw new InvalidOperationException("Unable to fetch roles.");
        RoleList = roles;
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
          Assigned = this.Assigned,
          RoleId = this.Role
        };
        dal.Insert(item);
        TimeStamp = item.LastChanged ?? [];
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
        var item = dal.Fetch(projectId, ResourceId) ?? throw new DataNotFoundException("Assignment");
        item.Assigned = this.Assigned;
        item.RoleId = Role;
        item.LastChanged = TimeStamp;
        dal.Update(item);
        TimeStamp = item.LastChanged ?? [];
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