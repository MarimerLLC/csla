using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  ///
  /// </summary>
  public static class TypeWithOperationsShouldBePartialAnalyzerConstants
  {
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.TypeWithOperationsShouldBePartial_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.TypeWithOperationsShouldBePartial_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  ///
  /// </summary>
  public static class TypeWithOperationsShouldBePartialAddPartialCodeFixConstants
  {
    /// <summary>
    ///
    /// </summary>
    public static string AddPartialDescription => Resources.TypeWithOperationsShouldBePartial_AddPartialDescription;
  }
}
