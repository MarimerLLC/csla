using System;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda();

      this.Run(() => x.Save());
    }

    private AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda Run(
      Func<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda> code)
    {
      return code();
    }
  }
}