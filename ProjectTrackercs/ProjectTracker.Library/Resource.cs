using Csla;
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

    private static PropertyInfo<int> IdProperty = new PropertyInfo<int>("Id");
    private int _id = IdProperty.DefaultValue;
    public int Id
    {
      get
      {
        return GetProperty<int>(IdProperty, _id);
      }
    }

    private static PropertyInfo<string> LastNameProperty = new PropertyInfo<string>("LastName", "Last name");
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

    private static PropertyInfo<string> FirstNameProperty = new PropertyInfo<string>("FirstName", "First name");
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

    private static PropertyInfo<string> FullNameProperty = new PropertyInfo<string>("FullName", "Full name");
    public string FullName
    {
      get
      {
        return LastName + ", " + FirstName;
      }
    }

    private static PropertyInfo<ResourceAssignments> AssignmentsProperty = new PropertyInfo<ResourceAssignments>("Assignments");
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

    public static bool CanAddObject()
    {
      return Csla.ApplicationContext.User.IsInRole("ProjectManager");
    }

    public static bool CanGetObject()
    {
      return true;
    }

    public static bool CanDeleteObject()
    {
      bool result = false;
      if (Csla.ApplicationContext.User.IsInRole("ProjectManager"))
        result = true;
      if (Csla.ApplicationContext.User.IsInRole("Administrator"))
        result = true;
      return result;
    }

    public static bool CanEditObject()
    {
      return Csla.ApplicationContext.User.IsInRole("ProjectManager");
    }

    #endregion

    #region  Factory Methods

    public static Resource NewResource()
    {
      if (!(CanAddObject()))
        throw new System.Security.SecurityException("User not authorized to add a resource");
      return DataPortal.Create<Resource>();
    }

    public static void DeleteResource(int id)
    {
      if (!(CanDeleteObject()))
        throw new System.Security.SecurityException("User not authorized to remove a resource");
      DataPortal.Delete(new SingleCriteria<Resource, int>(id));
    }

    public static Resource GetResource(int id)
    {
      if (!(CanGetObject()))
        throw new System.Security.SecurityException("User not authorized to view a resource");
      return DataPortal.Fetch<Resource>(new SingleCriteria<Resource, int>(id));
    }

    public override Resource Save()
    {
      if (IsDeleted && !(CanDeleteObject()))
        throw new System.Security.SecurityException("User not authorized to remove a resource");
      else if (IsNew && !(CanAddObject()))
        throw new System.Security.SecurityException("User not authorized to add a resource");
      else if (!(CanEditObject()))
        throw new System.Security.SecurityException("User not authorized to update a resource");
      return base.Save();
    }

    private Resource()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    [RunLocal()]
    protected override void DataPortal_Create()
    {
      // nothing to initialize
      ValidationRules.CheckRules();
    }

    private void DataPortal_Fetch(SingleCriteria<Resource, int> criteria)
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(Database.PTrackerConnection))
      {
        var data = (from r in ctx.DataContext.Resources
                    where r.Id == criteria.Value
                    select r).Single();
        _id = data.Id;
        _lastName = data.LastName;
        _firstName = data.FirstName;
        _timestamp = data.LastChanged.ToArray();

        SetProperty<ResourceAssignments>(AssignmentsProperty, ResourceAssignments.GetResourceAssignments(data.Assignments.ToArray()));
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(Database.PTrackerConnection))
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
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(Database.PTrackerConnection))
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
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(Database.PTrackerConnection))
      {
        ctx.DataContext.deleteResource(_id);
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
        using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(Database.PTrackerConnection))
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