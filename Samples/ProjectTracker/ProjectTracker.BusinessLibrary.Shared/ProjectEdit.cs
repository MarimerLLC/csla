using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Data;
using Csla.Security;
using Csla.Serialization;
using System.ComponentModel;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectEdit : CslaBaseTypes.BusinessBase<ProjectEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> IdProperty = 
      RegisterProperty<int>(c => c.Id);
    [Display(Name = "Project id")]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = 
      RegisterProperty<string>(c => c.Name);
    [Display(Name = "Project name")]
    [Required]
    [StringLength(50)]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<DateTime?> StartedProperty = RegisterProperty<DateTime?>(c => c.Started);
    public DateTime? Started
    {
      get { return GetProperty(StartedProperty); }
      set { SetProperty(StartedProperty, value); }
    }

    public static readonly PropertyInfo<DateTime?> EndedProperty = RegisterProperty<DateTime?>(c => c.Ended);
    public DateTime? Ended
    {
      get { return GetProperty(EndedProperty); }
      set { SetProperty(EndedProperty, value); }
    }

    public static readonly PropertyInfo<string> DescriptionProperty = 
      RegisterProperty<string>(c => c.Description);
    public string Description
    {
      get { return GetProperty(DescriptionProperty); }
      set { SetProperty(DescriptionProperty, value); }
    }

    public static readonly PropertyInfo<ProjectResources> ResourcesProperty = 
      RegisterProperty<ProjectResources>(p => p.Resources);
    public ProjectResources Resources
    {
      get { return LazyGetProperty(ResourcesProperty, DataPortal.CreateChild<ProjectResources>); }
      private set { LoadProperty(ResourcesProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
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
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, StartedProperty, "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, EndedProperty, "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, DescriptionProperty, "ProjectManager"));
      BusinessRules.AddRule(new NoDuplicateResource { PrimaryProperty = ResourcesProperty });
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(
        typeof(ProjectEdit), 
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.CreateObject, 
          "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "ProjectManager", "Administrator"));
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
      protected override void Execute(Csla.Rules.RuleContext context)
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
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (ProjectEdit)context.Target;

        var started = target.ReadProperty(StartedProperty);
        var ended = target.ReadProperty(EndedProperty);
        if (started.HasValue && ended.HasValue && started > ended || !started.HasValue && ended.HasValue)
          context.AddErrorResult("Start date can't be after end date");
      }
    }

    public static void NewProject(EventHandler<DataPortalResult<ProjectEdit>> callback)
    {
      ProjectGetter.CreateNewProject((o, e) =>
      {
        callback(o, new DataPortalResult<ProjectEdit>(e.Object.Project, e.Error, null));
      });
    }

    public static void GetProject(int id, EventHandler<DataPortalResult<ProjectEdit>> callback)
    {
      ProjectGetter.GetExistingProject(id, (o, e) =>
        {
          callback(o, new DataPortalResult<ProjectEdit>(e.Object.Project, e.Error, null));
        });
    }

    public static void Exists(int id, Action<bool> result)
    {
      var cmd = new ProjectExistsCommand(id);
      DataPortal.BeginExecute<ProjectExistsCommand>(cmd, (o, e) =>
      {
        if (e.Error != null)
          throw e.Error;
        else
          result(e.Object.ProjectExists);
      });
    }

    public static void DeleteProject(int id, EventHandler<DataPortalResult<ProjectEdit>> callback)
    {
      DataPortal.BeginDelete<ProjectEdit>(id, callback);
    }

#if !WINDOWS_PHONE
    public async static System.Threading.Tasks.Task<ProjectEdit> NewProjectAsync()
    {
      return await DataPortal.CreateAsync<ProjectEdit>();
    }

    public async static System.Threading.Tasks.Task<ProjectEdit> GetProjectAsync(int id)
    {
      return await DataPortal.FetchAsync<ProjectEdit>(id);
    }
#endif
#if FULL_DOTNET
    public static ProjectEdit NewProject()
    {
      return DataPortal.Create<ProjectEdit>();
    }

    public static ProjectEdit GetProject(int id)
    {
      return DataPortal.Fetch<ProjectEdit>(id);
    }

    public static void DeleteProject(int id)
    {
      DataPortal.Delete<ProjectEdit>(id);
    }

    public static bool Exists(int id)
    {
      var cmd = new ProjectExistsCommand(id);
      cmd = DataPortal.Execute<ProjectExistsCommand>(cmd);
      return cmd.ProjectExists;
    }

    [RunLocal]
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(int id)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
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
    }

    protected override void DataPortal_Insert()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
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
    }

    protected override void DataPortal_Update()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
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
        FieldManager.UpdateChildren(this);
      }
    }

    protected override void DataPortal_DeleteSelf()
    {
      using (BypassPropertyChecks)
        DataPortal_Delete(this.Id);
    }

    private void DataPortal_Delete(int id)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        Resources.Clear();
        FieldManager.UpdateChildren(this);
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
        dal.Delete(id);
      }
    }
#endif
  }
}