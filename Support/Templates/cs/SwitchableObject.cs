using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class SwitchableObject : BusinessBase<SwitchableObject>
  {
    #region Business Methods

    // TODO: add your own fields, properties and methods

    // example with private backing field
    private static PropertyInfo<int> IdProperty =
      RegisterProperty(new PropertyInfo<int>("Id"));
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    private static PropertyInfo<string> NameProperty =
      RegisterProperty(new PropertyInfo<string>("Name"));
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
      //AuthorizationRules.AllowEdit(typeof(SwitchableObject), "Role");
    }

    #endregion

    #region Root Factory Methods

    public static SwitchableObject NewSwitchableObject()
    {
      return DataPortal.Create<SwitchableObject>();
    }

    public static SwitchableObject GetSwitchableObject(int id)
    {
      return DataPortal.Fetch<SwitchableObject>(
        new SingleCriteria<SwitchableObject, int>(id));
    }

    public static void DeleteSwitchableObject(int id)
    {
      DataPortal.Delete<SwitchableObject>(new SingleCriteria<SwitchableObject, int>(id));
    }

    #endregion

    #region Child Factory Methods

    internal static SwitchableObject NewSwitchableChild()
    {
      return DataPortal.CreateChild<SwitchableObject>();
    }

    internal static SwitchableObject GetSwitchableChild(object childData)
    {
      return DataPortal.FetchChild<SwitchableObject>(childData);
    }

    private SwitchableObject()
    { /* Require use of factory methods */ }

    #endregion

    #region Root Data Access

    [RunLocal]
    protected override void DataPortal_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(SingleCriteria<SwitchableObject, int> criteria)
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
      DataPortal_Delete(new SingleCriteria<SwitchableObject, int>(this.Id));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(SingleCriteria<SwitchableObject, int> criteria)
    {
      // TODO: delete values
    }

    #endregion

    #region Child Data Access

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
