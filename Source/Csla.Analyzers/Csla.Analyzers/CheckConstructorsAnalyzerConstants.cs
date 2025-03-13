using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class PublicNoArgumentConstructorIsMissingConstants
  {
    public const string HasNonPublicNoArgumentConstructor = "HasNonPublicNoArgumentConstructor";
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.PublicNoArgumentConstructorIsMissing_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.PublicNoArgumentConstructorIsMissing_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class ConstructorHasParametersConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.ConstructorHasParameters_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.ConstructorHasParameters_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class FindBusinessObjectCreationConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindBusinessObjectCreationConstants_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindBusinessObjectCreationConstants_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class CheckConstructorsAnalyzerPublicConstructorCodeFixConstants
  {
    public static string AddPublicConstructorDescription => Resources.CheckConstructorsAnalyzerPublicConstructor_AddPublicConstructorDescription;
    /// <summary>
    /// 
    /// </summary>
    public static string UpdateNonPublicConstructorToPublicDescription => Resources.CheckConstructorsAnalyzerPublicConstructor_UpdateNonPublicConstructorToPublicDescription;
  }
}
