using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ResourceEdit : CslaBaseTypes.BusinessDocumentBase<ResourceEdit, ResourceAssignmentEdit>
  {
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial byte[] TimeStamp { get; set; }

    [Display(Name = "Resource id")]
    public partial int Id { get; set; }

    [Display(Name = "Last name")]
    [Required]
    [StringLength(50)]
    public partial string LastName { get; set; }

    [Display(Name = "First name")]
    [Required]
    [StringLength(50)]
    public partial string FirstName { get; set; }

    [Display(Name = "Full name")]
    public string FullName
    {
      get { return LastName + ", " + FirstName; }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    public async Task<ResourceAssignmentEdit> AssignToAsync(int projectId)
    {
      if (!Contains(projectId))
      {
        var creator = ApplicationContext.GetRequiredService<IDataPortal<ResourceAssignmentEditCreator>>();
        var project = await creator.FetchAsync(projectId);
        this.Add(project.Result);
        return project.Result;
      }
      else
      {
        throw new InvalidOperationException("Resource already assigned to project");
      }
    }

    public void Remove(int projectId)
    {
      var item = this.FirstOrDefault(r => r.ProjectId == projectId);
      if (item != null)
        Remove(item);
    }

    public bool Contains(int projectId)
    {
      return this.Any(r => r.ProjectId == projectId);
    }

    public bool ContainsDeleted(int projectId)
    {
      return DeletedList.Any(r => r.ProjectId == projectId);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, LastNameProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, FirstNameProperty, Security.Roles.ProjectManager));
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, Security.Roles.ProjectManager));
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, Security.Roles.ProjectManager));
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, Security.Roles.ProjectManager, Security.Roles.Administrator));
    }

    [RunLocal]
    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IResourceDal dal, [Inject] IAssignmentDal assignmentDal, [Inject] IChildDataPortal<ResourceAssignmentEdit> childPortal)
    {
      var data = dal.Fetch(id) ?? throw new DataNotFoundException("Resource");
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        FirstName = data.FirstName ?? string.Empty;
        LastName = data.LastName ?? string.Empty;
        TimeStamp = data.LastChanged ?? [];
      }
      using (LoadListMode)
      {
        var assignments = assignmentDal.FetchForResource(id);
        foreach (var item in assignments)
          Add(childPortal.FetchChild(item));
      }
    }

    [Insert]
    private void Insert([Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.ResourceDto
        {
          FirstName = this.FirstName,
          LastName = this.LastName
        };
        dal.Insert(item);
        Id = item.Id;
        TimeStamp = item.LastChanged ?? [];
      }
      FieldManager.UpdateChildren(this);
      Child_Update(this);
    }

    [Update]
    private void Update([Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.ResourceDto
        {
          Id = this.Id,
          FirstName = this.FirstName,
          LastName = this.LastName,
          LastChanged = this.TimeStamp
        };
        dal.Update(item);
        TimeStamp = item.LastChanged ?? [];
      }
      FieldManager.UpdateChildren(this);
      Child_Update(this);
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        Clear();
        Child_Update(this);
        Delete(this.Id, dal);
      }
    }

    [Delete]
    private void Delete(int id, [Inject] IResourceDal dal)
    {
      dal.Delete(id);
    }
  }
}
