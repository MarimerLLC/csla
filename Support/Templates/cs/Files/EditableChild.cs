using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class EditableChild : BusinessBase<EditableChild>
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
      //BusinessRules.AddRule(new Rule(), IdProperty);
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    [CreateChild]
    private void Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      BusinessRules.CheckRules();
    }

    [FetchChild]
    private void Fetch(object childData)
    {
      // TODO: load values
    }

    [InsertChild]
    private void Insert(object parent)
    {
      // TODO: insert values
    }

    [UpdateChild]
    private void Update(object parent)
    {
      // TODO: update values
    }

    [DeleteSelf]
    private void DeleteSelf(object parent)
    {
      // TODO: delete values
    }
  }
}
