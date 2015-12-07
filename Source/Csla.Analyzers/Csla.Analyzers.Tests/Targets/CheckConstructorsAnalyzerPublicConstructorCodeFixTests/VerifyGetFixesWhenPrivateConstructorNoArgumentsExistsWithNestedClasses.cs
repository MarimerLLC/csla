namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTestss
{
  public class VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses
    : BusinessBase<VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses>
  {
    private VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsWithNestedClasses() { }

    public class NestedClass
      : BusinessBase<NestedClass>
    {
      private NestedClass() { }
    }
  }
}