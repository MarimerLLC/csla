using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class SwitchableObject : BusinessBase<SwitchableObject>
  {
    // TODO: add your own fields, properties and methods
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

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

    [RunLocal]
    [Create]
    private void Create()
    {
      // TODO: load default values
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id)
    {
      // TODO: load values
    }

    [Insert]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Insert()
    {
      // TODO: insert values
    }

    [Update]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Update()
    {
      // TODO: update values
    }

    [DeleteSelf]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void DeleteSelf()
    {
      Delete(this.Id);
    }

    [Delete]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Delete(int id)
    {
      // TODO: delete values
    }

    [CreateChild]
    private void CreateChild()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      BusinessRules.CheckRules();
    }

    [FetchChild]
    private void FetchChild(object childData)
    {
      // TODO: load values
    }

    [InsertChild]
    private void InsertChild(object parent)
    {
      // TODO: insert values
    }

    [UpdateChild]
    private void UpdateChild(object parent)
    {
      // TODO: update values
    }

    [DeleteSelf]
    private void DeleteSelfChild(object parent)
    {
      // TODO: delete values
    }
  }
}
