using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class FindSaveAssignmentIssueAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindSaveAssignmentIssue_Title), Resources.ResourceManager, typeof(Resources));


    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindSaveAssignmentIssue_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class FindSaveAsyncAssignmentIssueAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindSaveAsyncAssignmentIssue_Title), Resources.ResourceManager, typeof(Resources));


    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindSaveAsyncAssignmentIssue_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants
  {
    public static string AddAssignmentDescription => Resources.FindSaveAssignmentIssue_AddAssignmentDescription;
  }
}
