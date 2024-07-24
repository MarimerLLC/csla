using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class FindSaveAssignmentIssueAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.FindSaveAssignmentIssue_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.FindSaveAssignmentIssue_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class FindSaveAsyncAssignmentIssueAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.FindSaveAsyncAssignmentIssue_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.FindSaveAsyncAssignmentIssue_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class FindSaveAssignmentIssueAnalyzerAddAssignmentCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string AddAssignmentDescription => Resources.FindSaveAssignmentIssue_AddAssignmentDescription;
  }
}
