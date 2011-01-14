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
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id, RelationshipTypes.PrivateField);
    private int _Id = IdProperty.DefaultValue;
    public int Id
    {
      get { return GetProperty(IdProperty, _Id); }
      set { SetProperty(IdProperty, ref _Id, value); }
    }

    // example with managed backing field
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      // TODO: add validation rules
      //BusinessRules.AddRule(new Rule(), IdProperty);
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    #endregion

    #region Root Factory Methods

    public static SwitchableObject NewSwitchableObject()
    {
      return DataPortal.Create<SwitchableObject>();
    }

    public static SwitchableObject GetSwitchableObject(int id)
    {
      return DataPortal.Fetch<SwitchableObject>(id);
    }

    public static void DeleteSwitchableObject(int id)
    {
      DataPortal.Delete<SwitchableObject>(id);
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

    private void DataPortal_Fetch(int criteria)
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
      DataPortal_Delete(this.Id);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(int criteria)
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
