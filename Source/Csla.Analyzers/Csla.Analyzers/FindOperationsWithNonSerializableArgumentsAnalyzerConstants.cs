using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class FindOperationsWithNonSerializableArgumentsConstants
  {
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.FindOperationsWithNonSerializableArguments_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.FindOperationsWithNonSerializableArguments_Message), Resources.ResourceManager, typeof(Resources));
  }
}
