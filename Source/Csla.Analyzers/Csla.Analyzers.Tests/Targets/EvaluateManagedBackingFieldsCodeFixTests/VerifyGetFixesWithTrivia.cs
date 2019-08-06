namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsCodeFixTests
{
  public class VerifyGetFixesWithTrivia
    : BusinessBase<VerifyGetFixesWithTrivia>
  {
    #region Properties
    private static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    #endregion

    public string Data => GetProperty(DataProperty);
  }
}