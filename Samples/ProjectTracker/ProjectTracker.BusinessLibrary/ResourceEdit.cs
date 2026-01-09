using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ResourceEdit : CslaBaseTypes.BusinessBase<ResourceEdit>
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

    public partial ResourceAssignments Assignments { get; private set; }

    public override string ToString()
    {
      return Id.ToString();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, LastNameProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, FirstNameProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new NoDuplicateProject { PrimaryProperty = AssignmentsProperty });
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, Security.Roles.ProjectManager));
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, Security.Roles.ProjectManager));
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, Security.Roles.ProjectManager, Security.Roles.Administrator));
    }

    protected override void OnChildChanged(Csla.Core.ChildChangedEventArgs e)
    {
      if (e.ChildObject is ResourceAssignments)
      {
        BusinessRules.CheckRules(AssignmentsProperty);
        OnPropertyChanged(AssignmentsProperty);
      }
      base.OnChildChanged(e);
    }

    private class NoDuplicateProject : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        if (context.Target is not ResourceEdit target)
          return;
        try
        {
          var assignments = target.Assignments;
          if (assignments is null)
            return;
          foreach (var item in assignments)
          {
            var count = assignments.Count(r => r.ProjectId == item.ProjectId);
            if (count > 1)
            {
              context.AddErrorResult("Duplicate projects not allowed");
              return;
            }
          }
        }
        catch
        {
          // Assignments may not be loaded yet
        }
      }
    }

    [RunLocal]
    [Create]
    private void Create([Inject] IChildDataPortal<ResourceAssignments> portal)
    {
      LoadProperty(AssignmentsProperty, portal.CreateChild());
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IResourceDal dal, [Inject] IChildDataPortal<ResourceAssignments> portal)
    {
      var data = dal.Fetch(id) ?? throw new DataNotFoundException("Resource");
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        FirstName = data.FirstName ?? string.Empty;
        LastName = data.LastName ?? string.Empty;
        TimeStamp = data.LastChanged ?? [];
        Assignments = portal.FetchChild(id);
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
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        Assignments?.Clear();
        FieldManager.UpdateChildren(this);
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
