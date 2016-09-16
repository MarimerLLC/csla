namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsCodeFixTests
{
  public class User 
    : BusinessBase<User>
  {
    PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }
  }
}