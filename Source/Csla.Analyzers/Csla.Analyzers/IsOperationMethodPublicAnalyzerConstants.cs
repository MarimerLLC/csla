using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class IsOperationMethodPublicAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.IsOperationMethodPublic_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public const string IsSealed = "IsSealed";
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.IsOperationMethodPublic_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string InternalDescription => Resources.IsOperationMethodPublic_InternalDescription;
    /// <summary>
    /// 
    /// </summary>
    public static string ProtectedDescription => Resources.IsOperationMethodPublic_ProtectedDescription;
    /// <summary>
    /// 
    /// </summary>
    public static string PrivateDescription => Resources.IsOperationMethodPublic_PrivateDescription;
  }
}
