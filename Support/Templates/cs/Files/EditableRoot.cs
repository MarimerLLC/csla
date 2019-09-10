using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableRoot : BusinessBase<EditableRoot>
  {
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
      // TODO: add validation rules
      base.AddBusinessRules();

      //BusinessRules.AddRule(new Rule(IdProperty));
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
      // omit this override if you have no defaults to set
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
  }
}
