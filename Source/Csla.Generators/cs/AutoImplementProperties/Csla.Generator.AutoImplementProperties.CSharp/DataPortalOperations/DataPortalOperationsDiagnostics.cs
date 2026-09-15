//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationsDiagnostics.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Diagnostics reported by the data portal analyzers</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations
{
  /// <summary>
  /// Diagnostic descriptors reported by the data portal operations and data
  /// portal extensions analyzers, which apply the same rules as the generators.
  /// The generators themselves report no diagnostics.
  /// </summary>
  internal static class DataPortalOperationsDiagnostics
  {
    private const string Category = "Csla.DataPortal";

    public const string DuplicateOperationNameId = "CSLADP002";
    public const string GenericExtensionsNotGeneratedId = "CSLADP003";
    public const string InvalidExtensionsTargetId = "CSLADP004";
    public const string ExtensionNameHiddenId = "CSLADP005";
    public const string InaccessibleParameterTypeId = "CSLADP006";
    public const string DuplicateExtensionMethodId = "CSLADP007";
    public const string InvalidExtensionPrefixId = "CSLADP008";
    public const string AmbiguousOperationNameId = "CSLADP009";
    public const string InvalidAsyncSuffixId = "CSLADP010";
    public const string SyncExtensionsNotGeneratedId = "CSLADP011";

    public static readonly DiagnosticDescriptor DuplicateOperationName = new(
      id: DuplicateOperationNameId,
      title: "Data portal operation methods share an operation name",
      messageFormat: "Methods '{0}' and '{1}' on '{2}' both map to data portal operation '{3}'; name-based dispatch uses '{0}'",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Info,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor GenericExtensionsNotGenerated = new(
      id: GenericExtensionsNotGeneratedId,
      title: "Data portal extensions are not generated for generic types",
      messageFormat: "Data portal extension methods are not generated for '{0}' because it is generic or nested in a generic type",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Info,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidExtensionsTarget = new(
      id: InvalidExtensionsTargetId,
      title: "Invalid target for DataPortalExtensions",
      messageFormat: "Data portal extension methods are not generated for '{0}': {1}",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ExtensionNameHidden = new(
      id: ExtensionNameHiddenId,
      title: "Generated data portal extension would be hidden",
      messageFormat: "The data portal extension method '{0}' for '{1}' is not generated because it would be hidden by the '{2}' instance method; set DataPortalExtensions.Prefix to give the generated methods distinct names",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InaccessibleParameterType = new(
      id: InaccessibleParameterTypeId,
      title: "Operation parameter type is not accessible",
      messageFormat: "The data portal extension method for '{0}' cannot be generated because parameter '{1}' has type '{2}', which is not accessible outside its containing type",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DuplicateExtensionMethod = new(
      id: DuplicateExtensionMethodId,
      title: "Generated data portal extension would be duplicated",
      messageFormat: "The data portal extension method '{0}' for '{1}' is not generated for '{2}' because another operation method already produces an extension method with the same signature",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidExtensionPrefix = new(
      id: InvalidExtensionPrefixId,
      title: "Invalid DataPortalExtensions prefix",
      messageFormat: "Data portal extension methods are not generated for '{0}' because the prefix '{1}' is not a valid C# identifier",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AmbiguousOperationName = new(
      id: AmbiguousOperationNameId,
      title: "Data portal operation methods are ambiguous",
      messageFormat: "Methods '{0}' and '{1}' on '{2}' both map to data portal operation '{3}' with the same number of injected parameters; the data portal cannot choose between them and reports an ambiguous match",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidAsyncSuffix = new(
      id: InvalidAsyncSuffixId,
      title: "Invalid data portal extensions async suffix",
      messageFormat: "The CslaDataPortalExtensionsAsyncSuffix value '{0}' is not valid in a C# identifier; generated async data portal extension methods use the 'Async' suffix",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true,
      customTags: WellKnownDiagnosticTags.CompilationEnd);

    public static readonly DiagnosticDescriptor SyncExtensionsNotGenerated = new(
      id: SyncExtensionsNotGeneratedId,
      title: "Synchronous data portal extensions are not generated without an async suffix",
      messageFormat: "Synchronous data portal extension methods are not generated because CslaDataPortalExtensionsAsyncSuffix is 'none', so they would have the same names as the async methods; set CslaGenerateSyncDataPortalExtensions to false",
      category: Category,
      defaultSeverity: DiagnosticSeverity.Warning,
      isEnabledByDefault: true,
      customTags: WellKnownDiagnosticTags.CompilationEnd);
  }
}
