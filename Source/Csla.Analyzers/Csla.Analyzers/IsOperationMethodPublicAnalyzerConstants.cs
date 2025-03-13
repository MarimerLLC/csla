using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class IsOperationMethodPublicAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.IsOperationMethodPublic_Title), Resources.ResourceManager, typeof(Resources));


    public const string IsSealed = "IsSealed";

    public static readonly LocalizableResourceString Message = new(nameof(Resources.IsOperationMethodPublic_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants
  {
    public static string InternalDescription => Resources.IsOperationMethodPublic_InternalDescription;

    public static string ProtectedDescription => Resources.IsOperationMethodPublic_ProtectedDescription;

    public static string PrivateDescription => Resources.IsOperationMethodPublic_PrivateDescription;
  }
}
