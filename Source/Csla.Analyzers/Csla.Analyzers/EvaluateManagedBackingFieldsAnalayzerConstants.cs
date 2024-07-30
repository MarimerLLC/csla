using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsAnalayzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.EvaluateManagedBackingFields_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.EvaluateManagedBackingFields_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string FixManagedBackingFieldDescription => Resources.EvaluateManagedBackingFields_FixManagedBackingFieldDescription;
  }
}
