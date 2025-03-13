using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class FindRefAndOutParametersInOperationsAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindRefAndOutParametersInOperations_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindRefAndOutParametersInOperations_Message), Resources.ResourceManager, typeof(Resources));
  }
}
