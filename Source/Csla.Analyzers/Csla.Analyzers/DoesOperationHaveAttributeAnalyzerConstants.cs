using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class DoesOperationHaveAttributeAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.DoesOperationHaveAttribute_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.DoesOperationHaveAttribute_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string AddAttributeAndUsingDescription => Resources.Shared_AddAttributeAndUsingDescription;
    /// <summary>
    /// 
    /// </summary>
    public static string AddAttributeDescription => Resources.Shared_AddAttributeDescription;
    /// <summary>
    /// 
    /// </summary>
    public const string CslaNamespace = "Csla";
  }
}
