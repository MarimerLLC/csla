using System;
using System.Linq;
using Csla;
using Csla.Data;
using Csla.Security;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class Project : BusinessBase<Project>
  {
    #region  Business Methods

    private static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    private static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(p=>p.Id);
    public Guid Id
    {
      get { return GetProperty(IdProperty); }
      set { LoadProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = 
      RegisterProperty<string>(p=>p.Name, "Project name");
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<SmartDate> StartedProperty = 
      RegisterProperty<SmartDate>(p=>p.Started);
    public string Started
    {
      get { return GetPropertyConvert<SmartDate, string>(StartedProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartedProperty, value); }
    }

    private static PropertyInfo<SmartDate> EndedProperty = 
      RegisterProperty<SmartDate>(p=>p.Ended, null ,new SmartDate(SmartDate.EmptyValue.MaxDate));
    public string Ended
    {
      get { return GetPropertyConvert<SmartDate, string>(EndedProperty); }
      set { SetPropertyConvert<SmartDate, string>(EndedProperty, value); }
    }

    private static PropertyInfo<string> DescriptionProperty = 
      RegisterProperty<string>(p=>p.Description);
    public string Description
    {
      get { return GetProperty(DescriptionProperty); }
      set { SetProperty(DescriptionProperty, value); }
    }

    private static PropertyInfo<ProjectResources> ResourcesProperty =
      RegisterProperty<ProjectResources>(p=>p.Resources);
    public ProjectResources Resources
    {
      get
      {
        if (!(FieldManager.FieldExists(ResourcesProperty)))
          LoadProperty(ResourcesProperty, ProjectResources.NewProjectResources());
        return GetProperty(ResourcesProperty);
      }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    #endregion

    #region  Business Rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(NameProperty, 50));

      BusinessRules.AddRule(new StartDateGTEndDate { PrimaryProperty = StartedProperty, AffectedProperties = { EndedProperty } });
      BusinessRules.AddRule(new StartDateGTEndDate { PrimaryProperty = EndedProperty, AffectedProperties = { StartedProperty } });
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

    #endregion

    #region  Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // add AuthorizationRules here
      AuthorizationRules.AllowWrite(NameProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(StartedProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(EndedProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(DescriptionProperty, "ProjectManager");
    }

    protected static void AddObjectAuthorizationRules()
    {
      // add object-level authorization rules here
      AuthorizationRules.AllowCreate(typeof(Project), "ProjectManager");
      AuthorizationRules.AllowEdit(typeof(Project), "ProjectManager");
      AuthorizationRules.AllowDelete(typeof(Project), "ProjectManager");
      AuthorizationRules.AllowDelete(typeof(Project), "Administrator");
    }

    #endregion

    #region  Factory Methods

    public static Project NewProject()
    {
      return DataPortal.Create<Project>();
    }

    public static Project GetProject(Guid id)
    {
      //return DataPortal.Fetch<Project>(new SingleCriteria<Project, Guid>(id));
      return DataPortal.Fetch<Project>(id);
    }

    public static void DeleteProject(Guid id)
    {
      DataPortal.Delete<Project>(new SingleCriteria<Project, Guid>(id));
    }

    private Project()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    [RunLocal()]
    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
      {
        Id = Guid.NewGuid();
        LoadProperty(StartedProperty, DateTime.Today);
        BusinessRules.CheckRules();
      }
    }

    //private void DataPortal_Fetch(SingleCriteria<Project, Guid> criteria)
    private void DataPortal_Fetch(Guid id)
    {
      using (var ctx = 
        ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.
        GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        // get project data
        var data = (from p in ctx.DataContext.Projects
                    where p.Id == id
                    select p).Single();
        using (BypassPropertyChecks)
        {
          Id = data.Id;
          Name = data.Name;
          LoadPropertyConvert<SmartDate, System.DateTime?>(
            StartedProperty, data.Started);
          LoadPropertyConvert<SmartDate, System.DateTime?>(
            EndedProperty, data.Ended);
          Description = data.Description;
          TimeStamp = data.LastChanged.ToArray();
        }

        // get child data
        LoadProperty(
          ResourcesProperty, 
          ProjectResources.GetProjectResources(
            data.Assignments.ToArray()));
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (var ctx = 
        ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.
        GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        // insert project data
        System.Data.Linq.Binary lastChanged = null;
        using (BypassPropertyChecks)
        {
          ctx.DataContext.addProject(
            Id,
            Name,
            ReadProperty(StartedProperty),
            ReadProperty(EndedProperty),
            Description,
            ref lastChanged);
          TimeStamp = lastChanged.ToArray();
        }
        // update child objects
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        // insert project data
        System.Data.Linq.Binary lastChanged = null;
        using (BypassPropertyChecks)
        {
          ctx.DataContext.updateProject(
              Id,
              Name,
              ReadProperty(StartedProperty),
              ReadProperty(EndedProperty),
              Description,
              TimeStamp,
              ref lastChanged);
          TimeStamp = lastChanged.ToArray();
        }
        // update child objects
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(
        new SingleCriteria<Project, Guid>(Id));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(SingleCriteria<Project, Guid> criteria)
    {
      using (var ctx = 
        ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.
        GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        // delete project data
        ctx.DataContext.deleteProject(criteria.Value);
        // reset child list field
        LoadProperty(ResourcesProperty, ProjectResources.NewProjectResources());
      }
    }

    #endregion

    #region  Exists

    public static bool Exists(Guid id)
    {
      return ExistsCommand.Exists(id);
    }

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

      public static bool Exists(Guid id)
      {
        var result = DataPortal.Execute<ExistsCommand>(new ExistsCommand { Id = id });
        return result.ProjectExists;
      }

      protected override void DataPortal_Execute()
      {
        using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          ProjectExists = ((from p in ctx.DataContext.Projects
                      where p.Id == Id
                      select p).Count() > 0);
        }
      }
    }

    #endregion

  }
}