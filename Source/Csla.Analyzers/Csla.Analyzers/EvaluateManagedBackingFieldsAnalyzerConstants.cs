using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.EvaluateManagedBackingFields_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.EvaluateManagedBackingFields_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsCodeFixConstants
  {
    public static string FixManagedBackingFieldDescription => Resources.EvaluateManagedBackingFields_FixManagedBackingFieldDescription;
  }
}
