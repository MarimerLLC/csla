// Portions of this analyzer are adapted from the NotDataPortalExtensionMethodUsedAnalyzer
// in Csla.DataPortalExtensions (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// Copyright (c) Stefan Ossendorf, licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports calls to the untyped, criteria-based methods of
  /// <c>IDataPortal&lt;T&gt;</c>, <c>IChildDataPortal&lt;T&gt;</c>, and
  /// <c>DataPortal&lt;T&gt;</c> when <c>T</c> is marked with
  /// <c>[DataPortalExtensions]</c>, meaning strongly typed extension
  /// methods are generated and should be used instead.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class UseGeneratedDataPortalExtensionAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor useExtensionRule =
      new(
        Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, UseGeneratedDataPortalExtensionAnalyzerConstants.Title,
        UseGeneratedDataPortalExtensionAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension, nameof(UseGeneratedDataPortalExtensionAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [useExtensionRule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      // Generated code must be skipped: the generated extension methods
      // themselves call the untyped data portal methods.
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(compilationContext =>
      {
        var compilation = compilationContext.Compilation;
        var extensionsAttribute = compilation.GetTypeByMetadataName("Csla.DataPortalExtensionsAttribute");
        if (extensionsAttribute is null)
        {
          return;
        }

        var symbols = new KnownSymbols(
          extensionsAttribute,
          compilation.GetTypeByMetadataName("Csla.IDataPortal`1"),
          compilation.GetTypeByMetadataName("Csla.IChildDataPortal`1"),
          compilation.GetTypeByMetadataName("Csla.DataPortal`1"));

        compilationContext.RegisterOperationAction(
          operationContext => AnalyzeInvocation(operationContext, symbols), OperationKind.Invocation);
      });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, KnownSymbols symbols)
    {
      var invocation = (IInvocationOperation)context.Operation;
      var method = invocation.TargetMethod;
      var containingType = method.ContainingType;
      if (containingType is null || containingType.TypeArguments.Length != 1)
      {
        return;
      }

      var definition = containingType.OriginalDefinition;
      var isDataPortalClass = SymbolEqualityComparer.Default.Equals(definition, symbols.DataPortal);
      var isRootPortal = isDataPortalClass || SymbolEqualityComparer.Default.Equals(definition, symbols.IDataPortal);
      var isChildPortal = isDataPortalClass || SymbolEqualityComparer.Default.Equals(definition, symbols.IChildDataPortal);
      if (!isRootPortal && !isChildPortal)
      {
        return;
      }

      if (!IsCriteriaMethod(method, isRootPortal, isChildPortal))
      {
        return;
      }

      context.CancellationToken.ThrowIfCancellationRequested();

      if (containingType.TypeArguments[0] is not INamedTypeSymbol businessType ||
        !HasDataPortalExtensionsAttribute(businessType, symbols.DataPortalExtensionsAttribute))
      {
        return;
      }

      context.ReportDiagnostic(Diagnostic.Create(
        useExtensionRule, GetLocation(invocation), businessType.Name, method.Name));
    }

    private static bool IsCriteriaMethod(IMethodSymbol method, bool isRootPortal, bool isChildPortal)
    {
      switch (method.Name)
      {
        case "Create":
        case "CreateAsync":
        case "Fetch":
        case "FetchAsync":
        case "Delete":
        case "DeleteAsync":
          return isRootPortal;
        case "Execute":
        case "ExecuteAsync":
          // Only the params object[] criteria overload, not Execute(T command)
          return isRootPortal && method.Parameters.Length == 1 && method.Parameters[0].IsParams;
        case "CreateChild":
        case "CreateChildAsync":
        case "FetchChild":
        case "FetchChildAsync":
          return isChildPortal;
        default:
          return false;
      }
    }

    private static bool HasDataPortalExtensionsAttribute(INamedTypeSymbol businessType, INamedTypeSymbol attributeType)
    {
      foreach (var attribute in businessType.GetAttributes())
      {
        if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
        {
          return true;
        }
      }

      return false;
    }

    private static Location GetLocation(IInvocationOperation invocation)
    {
      if (invocation.Syntax is InvocationExpressionSyntax invocationSyntax)
      {
        switch (invocationSyntax.Expression)
        {
          case MemberAccessExpressionSyntax memberAccess:
            return memberAccess.Name.GetLocation();
          case MemberBindingExpressionSyntax memberBinding:
            return memberBinding.Name.GetLocation();
        }
      }

      return invocation.Syntax.GetLocation();
    }

    private sealed class KnownSymbols(
      INamedTypeSymbol dataPortalExtensionsAttribute,
      INamedTypeSymbol? iDataPortal,
      INamedTypeSymbol? iChildDataPortal,
      INamedTypeSymbol? dataPortal)
    {
      public INamedTypeSymbol DataPortalExtensionsAttribute { get; } = dataPortalExtensionsAttribute;
      public INamedTypeSymbol? IDataPortal { get; } = iDataPortal;
      public INamedTypeSymbol? IChildDataPortal { get; } = iChildDataPortal;
      public INamedTypeSymbol? DataPortal { get; } = dataPortal;
    }
  }
}
