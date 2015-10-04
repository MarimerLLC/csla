using System;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlockCaller
  {
    public void Call()
    {
      this.Run(() =>
      {
        var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock();
        return x.Save();
      });
    }

    private AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock Run(
      Func<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaWithBlock> code)
    {
      return code();
    }
  }
}