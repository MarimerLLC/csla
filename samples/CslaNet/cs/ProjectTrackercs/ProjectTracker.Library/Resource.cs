using Csla;
using Csla.Security;
using Csla.Data;
using System;
using System.Linq;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class Resource : BusinessBase<Resource>
  {
    #region  Business Methods

    private static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(p=>p.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> LastNameProperty = 
      RegisterProperty<string>(p=>p.LastName, "Last name");
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    private static PropertyInfo<string> FirstNameProperty = 
      RegisterProperty<string>(p=>p.FirstName, "First name");
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    private static PropertyInfo<string> FullNameProperty = 
      RegisterProperty<string>(p=>p.FullName, "Full name");
    public string FullName
    {
      get { return LastName + ", " + FirstName; }
    }

    private static PropertyInfo<ResourceAssignments> AssignmentsProperty =
      RegisterProperty<ResourceAssignments>(p=>p.Assignments);
    public ResourceAssignments Assignments
    {
      get
      {
        if (!(FieldManager.FieldExists(AssignmentsProperty)))
          LoadProperty(AssignmentsProperty, ResourceAssignments.NewResourceAssignments());
        return GetProperty(AssignmentsProperty);
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
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, FirstNameProperty);
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength,
        new Csla.Validation.CommonRules.MaxLengthRuleArgs(FirstNameProperty, 50));

      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, LastNameProperty);
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength,
        new Csla.Validation.CommonRules.MaxLengthRuleArgs(LastNameProperty, 50));
    }

    #endregion

    #region  Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // add AuthorizationRules here
      AuthorizationRules.AllowWrite(LastNameProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(FirstNameProperty, "ProjectManager");
    }

    protected static void AddObjectAuthorizationRules()
    {
      // add object-level authorization rules here
      AuthorizationRules.AllowCreate(typeof(Resource), "ProjectManager");
      AuthorizationRules.AllowEdit(typeof(Resource), "ProjectManager");
      AuthorizationRules.AllowDelete(typeof(Resource), "ProjectManager");
      AuthorizationRules.AllowDelete(typeof(Resource), "Administrator");
    }

    #endregion

    #region  Factory Methods

    public static Resource NewResource()
    {
      return DataPortal.Create<Resource>();
    }

    public static void DeleteResource(int id)
    {
      DataPortal.Delete<Resource>(new SingleCriteria<Resource, int>(id));
    }

    public static Resource GetResource(int id)
    {
      return DataPortal.Fetch<Resource>(new SingleCriteria<Resource, int>(id));
    }

    private Resource()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch(SingleCriteria<Resource, int> criteria)
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        var data = (from r in ctx.DataContext.Resources
                    where r.Id == criteria.Value
                    select r).Single();
        using (BypassPropertyChecks)
        {
          Id = data.Id;
          LastName = data.LastName;
          FirstName = data.FirstName;
          TimeStamp = data.LastChanged.ToArray();
        }
        LoadProperty(AssignmentsProperty, ResourceAssignments.GetResourceAssignments(data.Assignments.ToArray()));
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        int? newId = null;
        System.Data.Linq.Binary newLastChanged = null;
        using (BypassPropertyChecks)
        {
          ctx.DataContext.addResource(
            LastName, FirstName, ref newId, ref newLastChanged);
          Id = System.Convert.ToInt32(newId);
          TimeStamp = newLastChanged.ToArray();
        }
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        System.Data.Linq.Binary newLastChanged = null;
        using (BypassPropertyChecks)
        {
          ctx.DataContext.updateResource(Id, LastName, FirstName, TimeStamp, ref newLastChanged);
          TimeStamp = newLastChanged.ToArray();
        }
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new SingleCriteria<Resource, int>(Id));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(SingleCriteria<Resource, int> criteria)
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        ctx.DataContext.deleteResource(criteria.Value);
      }
    }

    #endregion

    #region  Exists

    public static bool Exists(int id)
    {
      return ExistsCommand.Exists(id);
    }

    [Serializable()]
    private class ExistsCommand : CommandBase
    {
      private int _id;
      private bool _exists;

      public bool ResourceExists
      {
        get
        {
          return _exists;
        }
      }

      public static bool Exists(int id)
      {
        ExistsCommand result = null;
        result = DataPortal.Execute<ExistsCommand>(new ExistsCommand(id));
        return result.ResourceExists;
      }

      private ExistsCommand(int id)
      {
        _id = id;
      }

      protected override void DataPortal_Execute()
      {
        using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          _exists = (from p in ctx.DataContext.Resources
                     where p.Id == _id
                     select p).Count() > 0;
        }
      }
    }

    #endregion

  }
}