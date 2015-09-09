using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFixTests
{
  public class User : BusinessBase<User> { }

  public class VerifyGetFixes
  {
    public async Task UseUser()
    {
      var x = DataPortal.Fetch<User>();
      await x.SaveAsync();
    }
  }
}