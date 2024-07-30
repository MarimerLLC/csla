using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class PublicNoArgumentConstructorIsMissingConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public const string HasNonPublicNoArgumentConstructor = "HasNonPublicNoArgumentConstructor";
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.PublicNoArgumentConstructorIsMissing_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.PublicNoArgumentConstructorIsMissing_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class ConstructorHasParametersConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.ConstructorHasParameters_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.ConstructorHasParameters_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class FindBusinessObjectCreationConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.FindBusinessObjectCreationConstants_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.FindBusinessObjectCreationConstants_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class CheckConstructorsAnalyzerPublicConstructorCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string AddPublicConstructorDescription => Resources.CheckConstructorsAnalyzerPublicConstructor_AddPublicConstructorDescription;
    /// <summary>
    /// 
    /// </summary>
    public static string UpdateNonPublicConstructorToPublicDescription => Resources.CheckConstructorsAnalyzerPublicConstructor_UpdateNonPublicConstructorToPublicDescription;
  }
}
