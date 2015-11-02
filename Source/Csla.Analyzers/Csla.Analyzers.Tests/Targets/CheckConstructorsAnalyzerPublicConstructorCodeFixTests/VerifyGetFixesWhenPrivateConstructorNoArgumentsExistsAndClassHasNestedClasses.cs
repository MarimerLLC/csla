namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerPublicConstructorCodeFixTestss
{
  public class VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndClassHasNestedClasses 
    : BusinessBase<VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndClassHasNestedClasses>
  {
    private VerifyGetFixesWhenPrivateConstructorNoArgumentsExistsAndClassHasNestedClasses() { }

    public class NestedClass
      : BusinessBase<NestedClass>
    {
      private NestedClass() { }
    }
  }
}