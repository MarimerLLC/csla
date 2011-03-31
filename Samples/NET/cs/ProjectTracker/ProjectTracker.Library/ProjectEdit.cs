using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Data;
using Csla.Security;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectEdit : BusinessBase<ProjectEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { LoadProperty(IdProperty, value); }
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

    public static readonly PropertyInfo<SmartDate> StartedProperty = 
      RegisterProperty<SmartDate>(c => c.Started,
      new SmartDate(SmartDate.EmptyValue.MaxDate));
    public string Started
    {
      get { return GetPropertyConvert<SmartDate, string>(StartedProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartedProperty, value); }
    }

    public static readonly PropertyInfo<SmartDate> EndedProperty = 
      RegisterProperty<SmartDate>(p => p.Ended, null, 
      new SmartDate(SmartDate.EmptyValue.MaxDate));
    public string Ended
    {
      get { return GetPropertyConvert<SmartDate, string>(EndedProperty); }
      set { SetPropertyConvert<SmartDate, string>(EndedProperty, value); }
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
      get
      {
        if (!(FieldManager.FieldExists(ResourcesProperty)))
          LoadProperty(ResourcesProperty, DataPortal.CreateChild<ProjectResources>());
        return GetProperty(ResourcesProperty);
      }
      private set { LoadProperty(ResourcesProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new StartDateGTEndDate { PrimaryProperty = StartedProperty, AffectedProperties = { EndedProperty } });
      BusinessRules.AddRule(new StartDateGTEndDate { PrimaryProperty = EndedProperty, AffectedProperties = { StartedProperty } });

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, NameProperty, "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, StartedProperty, "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, EndedProperty, "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, DescriptionProperty, "ProjectManager"));
    }

    protected static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ProjectEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "ProjectManager", "Administrator"));
    }

    private class StartDateGTEndDate : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (ProjectEdit)context.Target;

        var started = target.ReadProperty(StartedProperty);
        var ended = target.ReadProperty(EndedProperty);
        if (started > ended)
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

#if !SILVERLIGHT
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
          if (data.Started.HasValue)
            LoadProperty(StartedProperty, data.Started);
          else
            Started = string.Empty;
          if (data.Ended.HasValue)
            LoadProperty(EndedProperty, data.Ended);
          else
            Ended = string.Empty;
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
            Started = ReadProperty(StartedProperty),
            Ended = ReadProperty(EndedProperty)
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
            Started = ReadProperty(StartedProperty),
            Ended = ReadProperty(EndedProperty),
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