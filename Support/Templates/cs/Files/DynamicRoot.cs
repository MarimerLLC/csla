using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class DynamicRoot : BusinessBase<DynamicRoot>
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

    [Fetch]
    private void Fetch(object rootData)
    {
	    MarkOld();
      // TODO: load values from rootData
    }

    [Insert]
    private void Insert()
    {
      // TODO: insert values
    }

    [Update]
    private void Update()
    {
      // TODO: update values
    }

    [DeleteSelf]
    private void DeleteSelf()
    {
      // TODO: delete values
    }
  }
}