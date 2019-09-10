using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class ReadOnlyRoot : ReadOnlyBase<ReadOnlyRoot>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    [Fetch]
    private void Fetch(int id)
    {
      // TODO: call DAL to load values into object
    }
  }
}
