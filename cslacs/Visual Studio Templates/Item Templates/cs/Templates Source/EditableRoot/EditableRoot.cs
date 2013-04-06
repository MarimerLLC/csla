using System;
using Csla;

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

    public static $safeitemname$ NewEditableRoot()
    {
      return DataPortal.Create<$safeitemname$>();
    }

    public static $safeitemname$ GetEditableRoot(int id)
    {
      return DataPortal.Fetch<$safeitemname$>(
        new SingleCriteria<$safeitemname$, int>(id));
    }

    public static void DeleteEditableRoot(int id)
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
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(SingleCriteria<$safeitemname$, int> criteria)
    {
      // TODO: load values
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      // TODO: insert values
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      // TODO: update values
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
    }

    #endregion
  }
}
