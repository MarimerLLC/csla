namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixTests
{
  public class User : BusinessBase<User> { }

  public class VerifyGetFixes
  {
    public void UseUser()
    {
      var x = DataPortal.Fetch<User>();
      x.Save();
    }
  }
}