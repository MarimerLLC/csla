namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenFieldIsNotUsedByPropertyInfoManagement
    : BusinessBase<WalkWhenFieldIsNotUsedByPropertyInfoManagement>
  {
    public static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data { get; set; }
  }
}
