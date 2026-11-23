using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Resources for <see cref="MigrateToNotNullPropertyAnalyzer"/>.
  /// </summary>
  public static class MigrateToNotNullPropertyAnalyzerConstants
  {
    /// <summary>
    /// The title of the diagnostic for a registration that can migrate with Fix All.
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.MigrateToNotNullPropertyAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The message of the diagnostic for a registration that can migrate with Fix All; <c>{0}</c> is the property name.
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.MigrateToNotNullPropertyAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The title of the diagnostic for a registration to migrate after review.
    /// </summary>
    public static readonly LocalizableResourceString ReviewTitle = new(nameof(Resources.MigrateToNotNullPropertyAnalyzer_ReviewTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The message of the diagnostic for a registration to migrate after review; <c>{0}</c> is the property name.
    /// </summary>
    public static readonly LocalizableResourceString ReviewMessage = new(nameof(Resources.MigrateToNotNullPropertyAnalyzer_ReviewMessage), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// Resources for the code fixes of <see cref="MigrateToNotNullPropertyAnalyzer"/>.
  /// </summary>
  public static class MigrateToNotNullPropertyCodeFixConstants
  {
    /// <summary>
    /// The title of the code fix for a registration that can migrate with Fix All.
    /// </summary>
    public static string Title => Resources.MigrateToNotNullPropertyCodeFix_Title;
    /// <summary>
    /// The title of the code fix for a registration to migrate after review.
    /// </summary>
    public static string ReviewTitle => Resources.MigrateToNotNullPropertyCodeFix_ReviewTitle;
  }
}
