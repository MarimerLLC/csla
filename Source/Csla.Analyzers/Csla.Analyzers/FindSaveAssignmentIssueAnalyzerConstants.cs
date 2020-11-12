namespace Csla.Analyzers
{
  public static class FindSaveAssignmentIssueAnalyzerConstants
  {
    public const string Title = "Find Save() Calls That Ignore the Result";
    public const string IdentifierText = "FindSaveAssignmentIssue";
    public const string Message = "Do not ignore the result of Save()";
  }

  public static class FindSaveAsyncAssignmentIssueAnalyzerConstants
  {
    public const string Title = "Find SaveAsync() Calls That Ignore the Result";
    public const string IdentifierText = "FindSaveAsyncAssignmentIssue";
    public const string Message = "Do not ignore the result of SaveAsync()";
  }

  public static class FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants
  {
    public const string AddAssignmentDescription = "Add assignment";
  }

  public static class FindSaveAssignmentIssueAnalyzerAddAsyncAssignmentCodeFixConstants
  {
    public const string AddAssignmentDescription = "Add assignment";
  }
}
