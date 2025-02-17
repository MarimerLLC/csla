using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class FindOperationsWithNonSerializableArgumentsConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.FindOperationsWithNonSerializableArguments_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.FindOperationsWithNonSerializableArguments_Message), Resources.ResourceManager, typeof(Resources));
  }
}
