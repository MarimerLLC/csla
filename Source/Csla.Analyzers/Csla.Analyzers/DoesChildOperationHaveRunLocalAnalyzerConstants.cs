using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class DoesChildOperationHaveRunLocalAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.DoesChildOperationHaveRunLocal_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new(nameof(Resources.DoesChildOperationHaveRunLocal_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants
  {
    public static string RemoveRunLocalDescription => Resources.DoesChildOperationHaveRunLocalRemoveAttribute_RemoveRunLocalDescription;
  }
}
