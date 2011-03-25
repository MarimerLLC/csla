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
  public class Project : BusinessBase<Project>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(c => c.Id);
    public Guid Id
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
      RegisterProperty<SmartDate>(c => c.Started);
    public string Started
    {
      get { return GetPropertyConvert<SmartDate, string>(StartedProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartedProperty, value); }
    }

    public static readonly PropertyInfo<SmartDate> EndedProperty = RegisterProperty<SmartDate>(p => p.Ended, null, new SmartDate(SmartDate.EmptyValue.MaxDate));
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
      Csla.Rules.BusinessRules.AddRule(typeof(Project), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(Project), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(Project), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "ProjectManager", "Administrator"));
    }

    private class StartDateGTEndDate : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (Project)context.Target;
        if (target.ReadProperty(StartedProperty) > target.ReadProperty(EndedProperty))
          context.AddErrorResult("Start date can't be after end date");
      }
    }

#if !SILVERLIGHT
    public static Project NewProject()
    {
      return DataPortal.Create<Project>();
    }

    public static Project GetProject(Guid id)
    {
      return DataPortal.Fetch<Project>(id);
    }

    public static void DeleteProject(Guid id)
    {
      DataPortal.Delete<Project>(new SingleCriteria<Project, Guid>(id));
    }
#endif

    public static void NewProject(EventHandler<DataPortalResult<Project>> callback)
    {
      DataPortal.BeginCreate<Project>(callback);
    }

    public static void GetProject(Guid id, EventHandler<DataPortalResult<Project>> callback)
    {
      DataPortal.BeginFetch<Project>(id, callback);
    }

    #region  Exists

    public static void Exists(Guid id, Action<bool> result)
    {
      var cmd = new ExistsCommand(id);
      DataPortal.BeginExecute<ExistsCommand>(cmd, (o, e) =>
        {
          if (e.Error != null)
            throw e.Error;
          else
            result(e.Object.ProjectExists);
        });
    }

#if !SILVERLIGHT
    public static bool Exists(Guid id)
    {
      var cmd = new ExistsCommand(id);
      cmd = DataPortal.Execute<ExistsCommand>(cmd);
      return cmd.ProjectExists;
    }
#endif

    [Serializable()]
    private class ExistsCommand : CommandBase<ExistsCommand>
    {
      public static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(c => c.Id);
      private Guid Id
      {
        get { return ReadProperty(IdProperty); }
        set { LoadProperty(IdProperty, value); }
      }

      public static PropertyInfo<bool> ResultProperty = RegisterProperty<bool>(c => c.ProjectExists);
      public bool ProjectExists
      {
        get { return ReadProperty(ResultProperty); }
        private set { LoadProperty(ResultProperty, value); }
      }

      public ExistsCommand(Guid id)
      {
        Id = id;
      }
    }

    #endregion
  }
}