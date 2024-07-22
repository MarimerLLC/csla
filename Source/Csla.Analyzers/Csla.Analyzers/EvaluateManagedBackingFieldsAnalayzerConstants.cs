using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class EvaluateManagedBackingFieldsAnalayzerConstants
  {
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.EvaluateManagedBackingFields_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.EvaluateManagedBackingFields_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class EvaluateManagedBackingFieldsCodeFixConstants
  {
    public static string FixManagedBackingFieldDescription => Resources.EvaluateManagedBackingFields_FixManagedBackingFieldDescription;
  }
}
