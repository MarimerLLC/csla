using System.Threading.Tasks;
using Csla.Rules;

namespace Csla.Analyzers.IntegrationTests
{
  public class ExecuteWithoutAdd  
    : BusinessRule
  {
    protected override void Execute(IRuleContext context)
    {
      // The parameter should have an error
      // because there are no Add...() calls.
    }
  }

  public class ExecuteWithAdd
    : BusinessRule
  {
    protected override void Execute(IRuleContext context)
    {
      context.AddDirtyProperty(null);
    }
  }

  public class AsynchronousRuleOldSchool
    : BusinessRule
  {
    protected override async void Execute(IRuleContext context)
    {
      context.AddDirtyProperty(null);
      // This method should have an error
      // because we are not inheriting from BusinessRuleAsync.
      await DummyAsync();
      context.Complete();
    }

    private static Task DummyAsync() => Task.CompletedTask;
  }

  public class AsynchronousRuleNewSchool
    : BusinessRuleAsync
  {
    protected override async Task ExecuteAsync(IRuleContext context)
    {
      context.AddDirtyProperty(null);
      await DummyAsync();
    }

    private static Task DummyAsync() => Task.CompletedTask;
  }

  public class CallingComplete
    : BusinessRuleAsync
  {
    protected override Task ExecuteAsync(IRuleContext context)
    {
      // This method should have an error
      // because we call Complete().
      #region keep this
      context.Complete();
      #endregion
      context.AddDirtyProperty(null);
      context.Complete(); context.Complete();
      // Keep these comments!
      context.Complete(); /* And this one */

      context.Complete();
      return Task.CompletedTask;
    }
  }
}
