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

    private byte[] _timestamp = new byte[8];

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(Resource), new PropertyInfo<int>("Id"));
    private int _id = IdProperty.DefaultValue;
    public int Id
    {
      get
      {
        return GetProperty<int>(IdProperty, _id);
      }
    }

    private static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(typeof(Resource), new PropertyInfo<string>("LastName", "Last name"));
    private string _lastName = LastNameProperty.DefaultValue;
    public string LastName
    {
      get
      {
        return GetProperty<string>(LastNameProperty, _lastName);
      }
      set
      {
        SetProperty<string>(LastNameProperty, ref _lastName, value);
      }
    }

    private static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(typeof(Resource), new PropertyInfo<string>("FirstName", "First name"));
    private string _firstName = FirstNameProperty.DefaultValue;
    public string FirstName
    {
      get
      {
        return GetProperty<string>(FirstNameProperty, _firstName);
      }
      set
      {
        SetProperty<string>(FirstNameProperty, ref _firstName, value);
      }
    }

    private static PropertyInfo<string> FullNameProperty = RegisterProperty<string>(typeof(Resource), new PropertyInfo<string>("FullName", "Full name"));
    public string FullName
    {
      get
      {
        return LastName + ", " + FirstName;
      }
    }

    private static PropertyInfo<ResourceAssignments> AssignmentsProperty = RegisterProperty<ResourceAssignments>(typeof(Resource), new PropertyInfo<ResourceAssignments>("Assignments"));
    public ResourceAssignments Assignments
    {
      get
      {
        if (!(FieldManager.FieldExists(AssignmentsProperty)))
        {
          SetProperty<ResourceAssignments>(AssignmentsProperty, ResourceAssignments.NewResourceAssignments());
        }
        return GetProperty<ResourceAssignments>(AssignmentsProperty);
      }
    }

    public override string ToString()
    {
      return _id.ToString();
    }

    #endregion

    #region  Validation Rules

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
        _id = data.Id;
        _lastName = data.LastName;
        _firstName = data.FirstName;
        _timestamp = data.LastChanged.ToArray();

        LoadProperty<ResourceAssignments>(AssignmentsProperty, ResourceAssignments.GetResourceAssignments(data.Assignments.ToArray()));
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        int? newId = null;
        System.Data.Linq.Binary newLastChanged = null;
        ctx.DataContext.addResource(_lastName, _firstName, ref newId, ref newLastChanged);
        _id = System.Convert.ToInt32(newId);
        _timestamp = newLastChanged.ToArray();
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        System.Data.Linq.Binary newLastChanged = null;
        ctx.DataContext.updateResource(_id, _lastName, _firstName, _timestamp, ref newLastChanged);
        _timestamp = newLastChanged.ToArray();
        FieldManager.UpdateChildren(this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new SingleCriteria<Resource, int>(_id));
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