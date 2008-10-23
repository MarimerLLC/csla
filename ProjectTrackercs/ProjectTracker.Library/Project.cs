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

    private byte[] _timestamp = new byte[8];

    private static PropertyInfo<Guid> IdProperty = 
      RegisterProperty(typeof(Project), new PropertyInfo<Guid>("Id"));
    [System.ComponentModel.DataObjectField(true, true)]
    public Guid Id
    {
      get { return GetProperty(IdProperty); }
    }

    private static PropertyInfo<string> NameProperty = 
      RegisterProperty(typeof(Project), new PropertyInfo<string>("Name", "Project name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<SmartDate> StartedProperty = 
      RegisterProperty(typeof(Project), new PropertyInfo<SmartDate>("Started"));
    public string Started
    {
      get { return GetPropertyConvert<SmartDate, string>(StartedProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartedProperty, value); }
    }

    private static PropertyInfo<SmartDate> EndedProperty = 
      RegisterProperty(typeof(Project), 
      new PropertyInfo<SmartDate>("Ended", "Ended",
        new SmartDate(SmartDate.EmptyValue.MaxDate)));
    public string Ended
    {
      get { return GetPropertyConvert<SmartDate, string>(EndedProperty); }
      set { SetPropertyConvert<SmartDate, string>(EndedProperty, value); }
    }

    private static PropertyInfo<string> DescriptionProperty = 
      RegisterProperty(typeof(Project), new PropertyInfo<string>("Description"));
    public string Description
    {
      get { return GetProperty(DescriptionProperty); }
      set { SetProperty(DescriptionProperty, value); }
    }

    private static PropertyInfo<ProjectResources> ResourcesProperty = 
      RegisterProperty(typeof(Project), new PropertyInfo<ProjectResources>("Resources"));
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

    #region  Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired,
                              new Csla.Validation.RuleArgs(NameProperty));
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength,
        new Csla.Validation.CommonRules.MaxLengthRuleArgs(NameProperty, 50));

      var args = new Csla.Validation.DecoratedRuleArgs(NameProperty);
      args["MaxLength"] = 50;
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength,
        args);


      ValidationRules.AddRule<Project>(StartDateGTEndDate<Project>, StartedProperty);
      ValidationRules.AddRule<Project>(StartDateGTEndDate<Project>, EndedProperty);

      ValidationRules.AddDependentProperty(StartedProperty, EndedProperty, true);
    }

    private static bool StartDateGTEndDate<T>(
      T target, Csla.Validation.RuleArgs e) where T : Project
    {
      if (target.ReadProperty(StartedProperty) > target.ReadProperty(EndedProperty))
      {
        e.Description = "Start date can't be after end date";
        return false;
      }
      else
      {
        return true;
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
      return DataPortal.Fetch<Project>(new SingleCriteria<Project, Guid>(id));
    }

    public static void DeleteProject(Guid id)
    {
      DataPortal.Delete(new SingleCriteria<Project, Guid>(id));
    }

    private Project()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    [RunLocal()]
    protected override void DataPortal_Create()
    {
      LoadProperty(IdProperty, Guid.NewGuid());
      LoadProperty(StartedProperty, DateTime.Today);
      //Started = System.Convert.ToString(System.DateTime.Today);
      var e = ReadProperty(EndedProperty);
      ValidationRules.CheckRules();
    }

    private void DataPortal_Fetch(SingleCriteria<Project, Guid> criteria)
    {
      using (var ctx = 
        ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.
        GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        // get project data
        var data = (from p in ctx.DataContext.Projects
                    where p.Id == criteria.Value
                    select p).Single();
        LoadProperty(IdProperty, data.Id);
        LoadProperty(NameProperty, data.Name);
        LoadPropertyConvert<SmartDate, System.DateTime?>(
          StartedProperty, data.Started);
        LoadPropertyConvert<SmartDate, System.DateTime?>(
          EndedProperty, data.Ended);
        LoadProperty(DescriptionProperty, data.Description);
        _timestamp = data.LastChanged.ToArray();

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
        ctx.DataContext.addProject(
          ReadProperty(IdProperty),
          ReadProperty(NameProperty),
          ReadProperty(StartedProperty),
          ReadProperty(EndedProperty),
          ReadProperty(DescriptionProperty),
          ref lastChanged);
        _timestamp = lastChanged.ToArray();
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
        ctx.DataContext.updateProject(
          ReadProperty(IdProperty),
          ReadProperty(NameProperty),
          ReadProperty(StartedProperty),
          ReadProperty(EndedProperty),
          ReadProperty(DescriptionProperty),
          _timestamp,
          ref lastChanged);
        _timestamp = lastChanged.ToArray();
        // update child objects
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(
        new SingleCriteria<Project, Guid>(ReadProperty(IdProperty)));
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
    private class ExistsCommand : CommandBase
    {
      private Guid _id;
      private bool _exists;

      public bool ProjectExists
      {
        get { return _exists; }
      }

      public static bool Exists(Guid id)
      {
        ExistsCommand result = null;
        result = DataPortal.Execute<ExistsCommand>(
          new ExistsCommand(id));
        return result.ProjectExists;
      }

      private ExistsCommand(Guid id)
      {
        _id = id;
      }

      protected override void DataPortal_Execute()
      {
        using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          _exists = ((from p in ctx.DataContext.Projects
                      where p.Id == _id
                      select p).Count() > 0);
        }
      }
    }

    #endregion

  }
}