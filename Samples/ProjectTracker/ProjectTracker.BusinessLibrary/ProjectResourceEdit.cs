using Csla;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using Csla.Rules;
using ProjectTracker.Dal;
using System.Linq;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceEdit : BusinessBase<ProjectResourceEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty) ?? Array.Empty<byte>(); }
      set { SetProperty(TimeStampProperty, value); }
    }
#pragma warning restore CSLA0007

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
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty) ?? string.Empty; }
      private set { LoadProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty =
      RegisterProperty<string>(c => c.LastName);
    [Display(Name = "Last name")]
    public string LastName
    {
      get { return GetProperty(LastNameProperty) ?? string.Empty; }
      private set { LoadProperty(LastNameProperty, value); }
    }
#pragma warning restore CSLA0007

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

    public static readonly PropertyInfo<RoleList> RoleListProperty = RegisterProperty<RoleList>(nameof(RoleList));
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public RoleList RoleList
    {
      get => GetProperty(RoleListProperty) ?? throw new InvalidOperationException("Role list has not been loaded.");
      private set => LoadProperty(RoleListProperty, value);
    }
#pragma warning restore CSLA0007

    public string RoleName
    {
      get
      {
        var list = RoleList;
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
        LoadProperty(AssignedProperty, DateTime.Today);
        var person = dal.Fetch(resourceId) ?? throw new DataNotFoundException("Resource");
        FirstName = person.FirstName;
        LastName = person.LastName;
        var roles = rolePortal!.Fetch() ?? throw new InvalidOperationException("Unable to fetch roles.");
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
        LoadProperty(AssignedProperty, data.Assigned);
        TimeStamp = data.LastChanged;
        var person = dal.Fetch(data.ResourceId) ?? throw new DataNotFoundException("Resource");
        FirstName = person.FirstName;
        LastName = person.LastName;
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
        var item = dal.Fetch(projectId, ResourceId) ?? throw new DataNotFoundException("Assignment");
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