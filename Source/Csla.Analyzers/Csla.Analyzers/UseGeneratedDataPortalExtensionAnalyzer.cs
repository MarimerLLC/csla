// Portions of this analyzer are adapted from the NotDataPortalExtensionMethodUsedAnalyzer
// in Csla.DataPortalExtensions (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// Copyright (c) Stefan Ossendorf, licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
  /// methods are generated and should be used instead. The diagnostic is
  /// only reported when the generator produces an extension method that
  /// can replace the call.
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
          compilation.GetTypeByMetadataName("Csla.DataPortal`1"),
          compilation.GetTypeByMetadataName("Csla.NoDataPortalExtensionAttribute"),
          compilation.GetTypeByMetadataName("Csla.InjectAttribute"),
          compilation.GetTypeByMetadataName("Csla.Core.ICslaObject"));

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
        GetDataPortalExtensionsAttribute(businessType, symbols.DataPortalExtensionsAttribute) is not { } attribute)
      {
        return;
      }

      if (!HasGeneratedExtension(businessType, attribute, method.Name, GetCriteriaCount(invocation), symbols))
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

    private static AttributeData? GetDataPortalExtensionsAttribute(INamedTypeSymbol businessType, INamedTypeSymbol attributeType)
    {
      foreach (var attribute in businessType.GetAttributes())
      {
        if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
        {
          return attribute;
        }
      }

      return null;
    }

    /// <summary>
    /// Mirrors the data portal extensions generator: returns true when an
    /// extension method is generated for an operation method of the kind
    /// called that accepts the number of criteria values passed.
    /// </summary>
    private static bool HasGeneratedExtension(INamedTypeSymbol businessType, AttributeData attribute, string portalMethodName, int? criteriaCount, KnownSymbols symbols)
    {
      if (businessType.IsAbstract || symbols.ICslaObject is null ||
        !businessType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, symbols.ICslaObject)))
      {
        return false;
      }

      for (var type = businessType; type is not null; type = type.ContainingType)
      {
        if (type.TypeParameters.Length > 0)
        {
          return false;
        }
      }

      if (IsPrivate(businessType))
      {
        return false;
      }

      var prefix = attribute.NamedArguments
        .Where(a => a.Key == "Prefix")
        .Select(a => a.Value.Value as string)
        .FirstOrDefault() ?? string.Empty;
      if (prefix.Length > 0 && !SyntaxFacts.IsValidIdentifier(prefix))
      {
        return false;
      }

      var isAsync = portalMethodName.EndsWith("Async", StringComparison.Ordinal);
      var kind = isAsync ? portalMethodName.Substring(0, portalMethodName.Length - "Async".Length) : portalMethodName;
      var receiver = kind is "CreateChild" or "FetchChild" ? symbols.IChildDataPortal : symbols.IDataPortal;
      var hiddenNames = receiver is null
        ? ImmutableHashSet<string>.Empty
        : receiver.GetMembers().Select(m => m.Name).ToImmutableHashSet();
      var operationAttributeName = $"Csla.{kind}Attribute";

      foreach (var operation in businessType.GetMembers().OfType<IMethodSymbol>())
      {
        if (operation.MethodKind != MethodKind.Ordinary || operation.IsStatic || operation.IsGenericMethod)
        {
          continue;
        }

        var attributes = operation.GetAttributes();
        if (!attributes.Any(a => a.AttributeClass?.ToDisplayString() == operationAttributeName) ||
          attributes.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, symbols.NoDataPortalExtensionAttribute)))
        {
          continue;
        }

        // ref/out parameters and a single object[] parameter are only invoked through reflection
        if (operation.Parameters.Any(p => p.RefKind is RefKind.Ref or RefKind.Out) ||
          (operation.Parameters.Length == 1 && operation.Parameters[0].Type is IArrayTypeSymbol { Rank: 1, ElementType.SpecialType: SpecialType.System_Object }))
        {
          continue;
        }

        var criteria = operation.Parameters.Where(p => !IsInjected(p, symbols.InjectAttribute)).ToList();
        if (criteria.Any(p => IsPrivate(p.Type)))
        {
          continue;
        }

        var baseName = operation.Name;
        if (baseName.EndsWith("Async", StringComparison.Ordinal) && baseName.Length > "Async".Length)
        {
          baseName = baseName.Substring(0, baseName.Length - "Async".Length);
        }

        if (hiddenNames.Contains(prefix + baseName + (isAsync ? "Async" : string.Empty)))
        {
          continue;
        }

        if (criteriaCount is { } count)
        {
          var required = criteria.Count(p => !p.HasExplicitDefaultValue && !p.IsParams);
          var hasParams = criteria.Count > 0 && criteria[criteria.Count - 1].IsParams;
          if (count < required || (!hasParams && count > criteria.Count))
          {
            continue;
          }
        }

        return true;
      }

      return false;
    }

    /// <summary>
    /// The number of criteria values passed to the data portal method, or
    /// null when they are passed as an existing array.
    /// </summary>
    private static int? GetCriteriaCount(IInvocationOperation invocation)
    {
      if (invocation.Arguments.Length == 0)
      {
        return 0;
      }

      var argument = invocation.Arguments[invocation.Arguments.Length - 1];
      if (argument.ArgumentKind == ArgumentKind.ParamArray && argument.Value is IArrayCreationOperation { Initializer: { } initializer })
      {
        return initializer.ElementValues.Length;
      }

      return null;
    }

    private static bool IsInjected(IParameterSymbol parameter, INamedTypeSymbol? injectAttribute)
    {
      if (injectAttribute is null)
      {
        return false;
      }

      foreach (var attribute in parameter.GetAttributes())
      {
        for (var type = attribute.AttributeClass; type is not null; type = type.BaseType)
        {
          if (SymbolEqualityComparer.Default.Equals(type, injectAttribute))
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// The type, or a type it is composed of, is not accessible outside its containing type.
    /// </summary>
    private static bool IsPrivate(ITypeSymbol type)
    {
      switch (type)
      {
        case IArrayTypeSymbol array:
          return IsPrivate(array.ElementType);
        case INamedTypeSymbol named:
          for (var current = named; current is not null; current = current.ContainingType)
          {
            if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
            {
              return true;
            }
          }

          return named.TypeArguments.Any(IsPrivate);
        default:
          return false;
      }
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
      INamedTypeSymbol? dataPortal,
      INamedTypeSymbol? noDataPortalExtensionAttribute,
      INamedTypeSymbol? injectAttribute,
      INamedTypeSymbol? iCslaObject)
    {
      public INamedTypeSymbol DataPortalExtensionsAttribute { get; } = dataPortalExtensionsAttribute;
      public INamedTypeSymbol? IDataPortal { get; } = iDataPortal;
      public INamedTypeSymbol? IChildDataPortal { get; } = iChildDataPortal;
      public INamedTypeSymbol? DataPortal { get; } = dataPortal;
      public INamedTypeSymbol? NoDataPortalExtensionAttribute { get; } = noDataPortalExtensionAttribute;
      public INamedTypeSymbol? InjectAttribute { get; } = injectAttribute;
      public INamedTypeSymbol? ICslaObject { get; } = iCslaObject;
    }
  }
}
