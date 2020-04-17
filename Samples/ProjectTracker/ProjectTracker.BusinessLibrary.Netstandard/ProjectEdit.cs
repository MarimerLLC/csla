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
  public class ProjectEdit : CslaBaseTypes.BusinessBase<ProjectEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = 
      RegisterProperty<byte[]>(nameof(TimeStamp));
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> IdProperty = 
      RegisterProperty<int>(nameof(Id));
    [Display(Name = "Project id")]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = 
      RegisterProperty<string>(nameof(Name));
    [Display(Name = "Project name")]
    [Required]
    [StringLength(50)]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<DateTime?> StartedProperty = 
      RegisterProperty<DateTime?>(nameof(Started));
    public DateTime? Started
    {
      get { return GetProperty(StartedProperty); }
      set { SetProperty(StartedProperty, value); }
    }

    public static readonly PropertyInfo<DateTime?> EndedProperty = 
      RegisterProperty<DateTime?>(nameof(Ended));
    public DateTime? Ended
    {
      get { return GetProperty(EndedProperty); }
      set { SetProperty(EndedProperty, value); }
    }

    public static readonly PropertyInfo<string> DescriptionProperty = 
      RegisterProperty<string>(nameof(Description));
    public string Description
    {
      get { return GetProperty(DescriptionProperty); }
      set { SetProperty(DescriptionProperty, value); }
    }

    public static readonly PropertyInfo<ProjectResources> ResourcesProperty = 
      RegisterProperty<ProjectResources>(nameof(Resources));
    public ProjectResources Resources
    {
      get { return GetProperty(ResourcesProperty); }
      private set { LoadProperty(ResourcesProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      //BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(
        new StartDateGTEndDate { 
          PrimaryProperty = StartedProperty, 
          AffectedProperties = { EndedProperty } });
      BusinessRules.AddRule(
        new StartDateGTEndDate { 
          PrimaryProperty = EndedProperty, 
          AffectedProperties = { StartedProperty } });

      BusinessRules.AddRule(
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.WriteProperty, 
          NameProperty, 
          "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(
        Csla.Rules.AuthorizationActions.WriteProperty, StartedProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(
        Csla.Rules.AuthorizationActions.WriteProperty, EndedProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(
        Csla.Rules.AuthorizationActions.WriteProperty, DescriptionProperty, Security.Roles.ProjectManager));
      BusinessRules.AddRule(new NoDuplicateResource { PrimaryProperty = ResourcesProperty });
    }

    [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(
        typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.CreateObject, 
          "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, Security.Roles.ProjectManager));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, Security.Roles.ProjectManager, Security.Roles.Administrator));
    }

    protected override void OnChildChanged(Csla.Core.ChildChangedEventArgs e)
    {
      if (e.ChildObject is ProjectResources)
      {
        BusinessRules.CheckRules(ResourcesProperty);
        OnPropertyChanged(ResourcesProperty);
      }
      base.OnChildChanged(e);
    }

    private class NoDuplicateResource : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        var target = (ProjectEdit)context.Target;
        foreach (var item in target.Resources)
        {
          var count = target.Resources.Count(r => r.ResourceId == item.ResourceId);
          if (count > 1)
          {
            context.AddErrorResult("Duplicate resources not allowed");
            return;
          }
        }
      }
    }

    private class StartDateGTEndDate : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        var target = (ProjectEdit)context.Target;

        var started = target.ReadProperty(StartedProperty);
        var ended = target.ReadProperty(EndedProperty);
        if (started.HasValue && ended.HasValue && started > ended || !started.HasValue && ended.HasValue)
          context.AddErrorResult("Start date can't be after end date");
      }
    }

    public async static Task<ProjectEdit> NewProjectAsync()
    {
      return await DataPortal.CreateAsync<ProjectEdit>();
    }

    public async static Task<ProjectEdit> GetProjectAsync(int id)
    {
      return await DataPortal.FetchAsync<ProjectEdit>(id);
    }

    public static ProjectEdit NewProject()
    {
      return DataPortal.Create<ProjectEdit>();
    }

    public static ProjectEdit GetProject(int id)
    {
      return DataPortal.Fetch<ProjectEdit>(id);
    }

    public static async Task DeleteProjectAsync(int id)
    {
      await DataPortal.DeleteAsync<ProjectEdit>(id);
    }

    public static async Task<bool> ExistsAsync(int id)
    {
      var cmd = await DataPortal.CreateAsync<ProjectExistsCommand>(id);
      cmd = await DataPortal.ExecuteAsync(cmd);
      return cmd.ProjectExists;
    }

    [Create]
    [RunLocal]
    private void Create()
    {
      LoadProperty(ResourcesProperty, DataPortal.CreateChild<ProjectResources>());
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IProjectDal dal)
    {
      var data = dal.Fetch(id);
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        Name = data.Name;
        Description = data.Description;
        Started = data.Started;
        Ended = data.Ended;
        TimeStamp = data.LastChanged;
        Resources = DataPortal.FetchChild<ProjectResources>(id);
      }
    }

    [Insert]
    private void Insert([Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.ProjectDto
        {
          Name = this.Name,
          Description = this.Description,
          Started = this.Started,
          Ended = this.Ended
        };
        dal.Insert(item);
        Id = item.Id;
        TimeStamp = item.LastChanged;
      }
      FieldManager.UpdateChildren(this);
    }

    [Update]
    private void Update([Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        var item = new ProjectTracker.Dal.ProjectDto
        {
          Id = this.Id,
          Name = this.Name,
          Description = this.Description,
          Started = this.Started,
          Ended = this.Ended,
          LastChanged = this.TimeStamp
        };
        dal.Update(item);
        TimeStamp = item.LastChanged;
      }
      FieldManager.UpdateChildren(this.Id);
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] IProjectDal dal)
    {
      using (BypassPropertyChecks)
      {
        Resources.Clear();
        FieldManager.UpdateChildren(this);
        Delete(this.Id, dal);
      }
    }

    [Delete]
    private void Delete(int id, [Inject] IProjectDal dal)
    {
      dal.Delete(id);
    }
  }
}