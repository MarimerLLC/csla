using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using System.ComponentModel;
using System.Threading.Tasks;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceEdit : CslaBaseTypes.BusinessBase<ResourceEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

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
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty = 
      RegisterProperty<string>(c => c.FirstName);
    [Display(Name = "First name")]
    [Required]
    [StringLength(50)]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    [Display(Name = "Full name")]
    public string FullName
    {
      get { return LastName + ", " + FirstName; }
    }

    public static readonly PropertyInfo<ResourceAssignments> AssignmentsProperty =
      RegisterProperty<ResourceAssignments>(c => c.Assignments);
    public ResourceAssignments Assignments
    {
      get { return GetProperty(AssignmentsProperty); }
      private set { LoadProperty(AssignmentsProperty, value); }
    }

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
        foreach (var item in target.Assignments)
        {
          var count = target.Assignments.Count(r => r.ProjectId == item.ProjectId);
          if (count > 1)
          {
            context.AddErrorResult("Duplicate projects not allowed");
            return;
          }
        }
      }
    }

    public static async Task<ResourceEdit> NewResourceEditAsync()
    {
      return await DataPortal.CreateAsync<ResourceEdit>();
    }

    public static async Task<ResourceEdit> GetResourceEditAsync(int id)
    {
      return await DataPortal.FetchAsync<ResourceEdit>(id);
    }

    public static async Task<bool> ExistsAsync(int id)
    {
      var cmd = await DataPortal.CreateAsync<ResourceExistsCommand>(id);
      cmd = await DataPortal.ExecuteAsync(cmd);
      return cmd.ResourceExists;
    }

    public static ResourceEdit NewResourceEdit()
    {
      return DataPortal.Create<ResourceEdit>();
    }

    public static ResourceEdit GetResourceEdit(int id)
    {
      return DataPortal.Fetch<ResourceEdit>(id);
    }

    public static void DeleteResourceEdit(int id)
    {
      DataPortal.Delete<ResourceEdit>(id);
    }

    public static async Task DeleteResourceEditAsync(int id)
    {
      await DataPortal.DeleteAsync<ResourceEdit>(id);
    }

    [RunLocal]
    [Create]
    private void Create()
    {
      LoadProperty(AssignmentsProperty, DataPortal.CreateChild<ResourceAssignments>());
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IResourceDal dal)
    {
        var data = dal.Fetch(id);
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        FirstName = data.FirstName;
        LastName = data.LastName;
        TimeStamp = data.LastChanged;
        Assignments = DataPortal.FetchChild<ResourceAssignments>(id);
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