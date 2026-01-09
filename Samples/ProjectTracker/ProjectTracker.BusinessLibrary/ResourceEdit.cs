using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ResourceEdit : CslaBaseTypes.BusinessBase<ResourceEdit>
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

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    [Display(Name = "Resource id")]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty = 
      RegisterProperty<string>(c => c.LastName);
    [Display(Name = "Last name")]
    [Required]
    [StringLength(50)]
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public string LastName
    {
      get { return GetProperty(LastNameProperty) ?? string.Empty; }
      set { SetProperty(LastNameProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty = 
      RegisterProperty<string>(c => c.FirstName);
    [Display(Name = "First name")]
    [Required]
    [StringLength(50)]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty) ?? string.Empty; }
      set { SetProperty(FirstNameProperty, value); }
    }
#pragma warning restore CSLA0007

    [Display(Name = "Full name")]
    public string FullName
    {
      get { return LastName + ", " + FirstName; }
    }

    public static readonly PropertyInfo<ResourceAssignments> AssignmentsProperty =
      RegisterProperty<ResourceAssignments>(c => c.Assignments);
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public ResourceAssignments Assignments
    {
      get { return GetProperty(AssignmentsProperty)!; }
      private set { LoadProperty(AssignmentsProperty, value); }
    }
#pragma warning restore CSLA0007

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
        var target = (ResourceEdit)context.Target;
        try
        {
          var assignments = target.Assignments;
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
      LoadProperty(AssignmentsProperty, portal!.CreateChild()!);
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IResourceDal dal, [Inject] IChildDataPortal<ResourceAssignments> portal)
    {
        var data = dal.Fetch(id) ?? throw new DataNotFoundException("Resource");
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        FirstName = data.FirstName;
        LastName = data.LastName;
        TimeStamp = data.LastChanged;
        var assignments = portal!.FetchChild(id)!;
        Assignments = assignments;
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
        TimeStamp = item.LastChanged;
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
        TimeStamp = item.LastChanged;
      }
      FieldManager.UpdateChildren(this);
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] IResourceDal dal)
    {
      using (BypassPropertyChecks)
      {
        Assignments.Clear();
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