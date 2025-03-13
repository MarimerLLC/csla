using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class PublicNoArgumentConstructorIsMissingConstants
  {
    public const string HasNonPublicNoArgumentConstructor = "HasNonPublicNoArgumentConstructor";

    public static readonly LocalizableResourceString Title = new(nameof(Resources.PublicNoArgumentConstructorIsMissing_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new(nameof(Resources.PublicNoArgumentConstructorIsMissing_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class ConstructorHasParametersConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.ConstructorHasParameters_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new(nameof(Resources.ConstructorHasParameters_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class FindBusinessObjectCreationConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindBusinessObjectCreationConstants_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindBusinessObjectCreationConstants_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class CheckConstructorsAnalyzerPublicConstructorCodeFixConstants
  {
    public static string AddPublicConstructorDescription => Resources.CheckConstructorsAnalyzerPublicConstructor_AddPublicConstructorDescription;

    public static string UpdateNonPublicConstructorToPublicDescription => Resources.CheckConstructorsAnalyzerPublicConstructor_UpdateNonPublicConstructorToPublicDescription;
  }
}
