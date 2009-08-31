using System;
using Csla;
using System.Collections.Generic;

namespace $rootnamespace$
{
  [Serializable]
  public class $safeitemname$ : BusinessBase<$safeitemname$>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with private backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(typeof($safeitemname$), new PropertyInfo<int>("Id"));
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(typeof($safeitemname$), new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<$childlist$> ChildListProperty = 
      RegisterProperty<$childlist$>(typeof($safeitemname$), new PropertyInfo<$childlist$>("ChildList", "Child list"));
    public $childlist$ ChildList
    {
      get { return GetProperty<$childlist$>(ChildListProperty); }
    }

    private static PropertyInfo<$childitem$> ChildProperty = 
      RegisterProperty(typeof($safeitemname$), new PropertyInfo<$childitem$>("Child", "Child"));
    public $childitem$ Child
    {
      get { return GetProperty<$childitem$>(ChildProperty); }
    }
    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //ValidationRules.AddRule(RuleMethod, "");
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowWrite("Name", "Role");
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowEdit(typeof($safeitemname$), "Role");
    }

    #endregion

    #region Factory Methods

    public static $safeitemname$ New$safeitemname$()
    {
      return DataPortal.Create<$safeitemname$>();
    }

    public static $safeitemname$ Get$safeitemname$(int id)
    {
      return DataPortal.Fetch<$safeitemname$>(
        new SingleCriteria<$safeitemname$, int>(id));
    }

    public static void Delete$safeitemname$(int id)
    {
      DataPortal.Delete(new SingleCriteria<$safeitemname$, int>(id));
    }

    private $safeitemname$()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    [RunLocal]
    protected override void DataPortal_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      LoadProperty(ChildListProperty, $childlist$.New$childlist$());
      LoadProperty(ChildProperty, $childitem$.New$childitem$());
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(SingleCriteria<$safeitemname$, int> criteria)
    {
      // TODO: load values
      LoadProperty(ChildListProperty, $childlist$.Get$childlist$(null));
      LoadProperty(ChildProperty, $childitem$.Get$childitem$(null));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      // TODO: insert values
      FieldManager.UpdateChildren(this);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      // TODO: update values
      FieldManager.UpdateChildren(this);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new SingleCriteria<$safeitemname$, int>(this.Id));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(SingleCriteria<$safeitemname$, int> criteria)
    {
      // TODO: delete values
      FieldManager.UpdateChildren(this);
    }

    #endregion
  }


  [Serializable]
  public class $childlist$ : BusinessListBase<$childlist$, $childitem$>
  {
    #region Factory Methods

    internal static $childlist$ New$childlist$()
    {
      return DataPortal.CreateChild<$childlist$>();
    }

    internal static $childlist$ Get$childlist$(
      object childData)
    {
      return DataPortal.FetchChild<$childlist$>(childData);
    }

    private $childlist$()
    { }

    #endregion

    #region Data Access

    private void Child_Fetch(object childData)
    {
      RaiseListChangedEvents = false;
      foreach (var child in (IList<object>)childData)
        this.Add($childitem$.Get$childitem$(child));
      RaiseListChangedEvents = true;
    }

    #endregion
  }


  [Serializable]
  public class $childitem$ : BusinessBase<$childitem$>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with private backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(typeof($childitem$), new PropertyInfo<int>("Id"));
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(typeof($childitem$), new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //ValidationRules.AddRule(RuleMethod, "");
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowWrite("Name", "Role");
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowEdit(typeof($childitem$), "Role");
    }

    #endregion

    #region Factory Methods

    internal static $childitem$ New$childitem$()
    {
      return DataPortal.CreateChild<$childitem$>();
    }

    internal static $childitem$ Get$childitem$(object childData)
    {
      return DataPortal.FetchChild<$childitem$>(childData);
    }

    private $childitem$()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    protected override void Child_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      base.Child_Create();
    }

    private void Child_Fetch(object childData)
    {
      // TODO: load values
    }

    private void Child_Insert(object parent)
    {
      // TODO: insert values
    }

    private void Child_Update(object parent)
    {
      // TODO: update values
    }

    private void Child_DeleteSelf(object parent)
    {
      // TODO: delete values
    }

    #endregion
  }
}
