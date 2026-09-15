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
  /// <c>DataPortal&lt;T&gt;</c> when <c>T</c> or its assembly is marked with
  /// <c>[DataPortalExtensions]</c>, meaning strongly typed extension
  /// methods are generated and should be used instead. The diagnostic is
  /// only reported when the generator produces an extension method that
  /// can replace the call.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class UseGeneratedDataPortalExtensionAnalyzer
    : DiagnosticAnalyzer
  {
    /// <summary>
    /// Diagnostic property holding the names of the generated extension
    /// methods that can replace the call, separated by semicolons.
    /// </summary>
    public const string ExtensionNamesProperty = "ExtensionNames";

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

        var globalOptions = compilationContext.Options.AnalyzerConfigOptionsProvider.GlobalOptions;
        var symbols = new KnownSymbols(
          GetAsyncSuffix(globalOptions),
          GenerateSync(globalOptions),
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
        GetPrefix(businessType, symbols) is not { } prefix)
      {
        return;
      }

      var extensionNames = GetGeneratedExtensionNames(businessType, prefix, method.Name, GetCriteriaArguments(invocation), context.Compilation, symbols);
      if (extensionNames.Count == 0)
      {
        return;
      }

      var properties = ImmutableDictionary<string, string?>.Empty.Add(ExtensionNamesProperty, string.Join(";", extensionNames));
      context.ReportDiagnostic(Diagnostic.Create(
        useExtensionRule, GetLocation(invocation), properties, businessType.Name, method.Name));
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

    /// <summary>
    /// Mirrors the data portal extensions generator: returns the prefix of the
    /// extension methods requested for the business type by its own
    /// [DataPortalExtensions] attribute or by the attribute on its assembly,
    /// or null when no extension methods are requested.
    /// </summary>
    private static string? GetPrefix(INamedTypeSymbol businessType, KnownSymbols symbols)
    {
      if (businessType.IsRecord ||
        FindAttribute(businessType.GetAttributes(), symbols.NoDataPortalExtensionAttribute) is not null)
      {
        return null;
      }

      var typeAttribute = FindAttribute(businessType.GetAttributes(), symbols.DataPortalExtensionsAttribute);
      var assemblyAttribute = FindAttribute(businessType.ContainingAssembly.GetAttributes(), symbols.DataPortalExtensionsAttribute);
      if (typeAttribute is null && assemblyAttribute is null)
      {
        return null;
      }

      return GetPrefix(typeAttribute) ?? GetPrefix(assemblyAttribute) ?? string.Empty;
    }

    private static AttributeData? FindAttribute(ImmutableArray<AttributeData> attributes, INamedTypeSymbol? attributeType)
    {
      foreach (var attribute in attributes)
      {
        if (attributeType is not null && SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
        {
          return attribute;
        }
      }

      return null;
    }

    private static string? GetPrefix(AttributeData? attribute)
    {
      if (attribute is null)
      {
        return null;
      }

      foreach (var namedArgument in attribute.NamedArguments)
      {
        if (namedArgument.Key == "Prefix")
        {
          return namedArgument.Value.Value as string ?? string.Empty;
        }
      }

      return null;
    }

    /// <summary>
    /// Mirrors the data portal extensions generator: returns the names of the
    /// extension methods generated for operation methods of the kind called
    /// that accept the criteria values passed.
    /// </summary>
    private static List<string> GetGeneratedExtensionNames(INamedTypeSymbol businessType, string prefix, string portalMethodName,
      ImmutableArray<IOperation>? criteriaArguments, Compilation compilation, KnownSymbols symbols)
    {
      var names = new List<string>();
      if (businessType.IsAbstract || symbols.ICslaObject is null ||
        !businessType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, symbols.ICslaObject)))
      {
        return names;
      }

      for (var type = businessType; type is not null; type = type.ContainingType)
      {
        if (type.TypeParameters.Length > 0)
        {
          return names;
        }
      }

      if (IsPrivate(businessType))
      {
        return names;
      }

      if (prefix.Length > 0 && !SyntaxFacts.IsValidIdentifier(prefix))
      {
        return names;
      }

      var isAsync = portalMethodName.EndsWith("Async", StringComparison.Ordinal);
      if (!isAsync && !symbols.GenerateSync)
      {
        return names;
      }

      var kind = isAsync ? portalMethodName.Substring(0, portalMethodName.Length - "Async".Length) : portalMethodName;
      var receiver = kind is "CreateChild" or "FetchChild" ? symbols.IChildDataPortal : symbols.IDataPortal;
      var hiddenNames = receiver is null
        ? ImmutableHashSet<string>.Empty
        : receiver.GetMembers().Select(m => m.Name).ToImmutableHashSet();
      var operationAttributeName = $"Csla.{kind}Attribute";

      // Operation methods of this kind that generated code can dispatch to;
      // ref/out parameters and a single object[] parameter are only invoked through reflection.
      var operations = businessType.GetMembers()
        .OfType<IMethodSymbol>()
        .Where(m => m.MethodKind == MethodKind.Ordinary && !m.IsStatic && !m.IsGenericMethod)
        .Where(m => m.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == operationAttributeName))
        .Where(m => !m.Parameters.Any(p => p.RefKind is RefKind.Ref or RefKind.Out))
        .Where(m => !(m.Parameters.Length == 1 && m.Parameters[0].Type is IArrayTypeSymbol { Rank: 1, ElementType.SpecialType: SpecialType.System_Object }))
        .Select(m => (Method: m, Criteria: m.Parameters.Where(p => !IsInjected(p, symbols.InjectAttribute)).ToList()))
        .ToList();

      foreach (var (operation, criteria) in operations)
      {
        if (operation.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, symbols.NoDataPortalExtensionAttribute)))
        {
          continue;
        }

        // Methods with the same criteria types differ only in injected parameters;
        // the generator only produces an extension for the one the runtime prefers.
        if (!IsPreferredAmongSameCriteria(operation, criteria, operations))
        {
          continue;
        }

        if (criteria.Any(p => IsPrivate(p.Type)))
        {
          continue;
        }

        var baseName = operation.Name;
        if (baseName.EndsWith("Async", StringComparison.Ordinal) && baseName.Length > "Async".Length)
        {
          baseName = baseName.Substring(0, baseName.Length - "Async".Length);
        }

        var name = prefix + baseName + (isAsync ? symbols.AsyncSuffix : string.Empty);
        if (hiddenNames.Contains(name))
        {
          continue;
        }

        if (criteriaArguments is { } arguments && !AcceptsArguments(criteria, arguments, compilation))
        {
          continue;
        }

        if (!names.Contains(name))
        {
          names.Add(name);
        }
      }

      return names;
    }

    private static bool IsPreferredAmongSameCriteria(IMethodSymbol operation, List<IParameterSymbol> criteria,
      List<(IMethodSymbol Method, List<IParameterSymbol> Criteria)> operations)
    {
      var injectCount = operation.Parameters.Length - criteria.Count;
      foreach (var (other, otherCriteria) in operations)
      {
        if (SymbolEqualityComparer.Default.Equals(other, operation) ||
          !otherCriteria.Select(p => p.Type).SequenceEqual(criteria.Select(p => p.Type), SymbolEqualityComparer.Default))
        {
          continue;
        }

        var otherInjectCount = other.Parameters.Length - otherCriteria.Count;
        if (otherInjectCount > injectCount)
        {
          return false;
        }

        // ties go to the first declared method
        if (otherInjectCount == injectCount &&
          operations.FindIndex(o => SymbolEqualityComparer.Default.Equals(o.Method, other)) <
          operations.FindIndex(o => SymbolEqualityComparer.Default.Equals(o.Method, operation)))
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// The criteria values can be passed to the criteria parameters of the
    /// generated extension method.
    /// </summary>
    private static bool AcceptsArguments(List<IParameterSymbol> criteria, ImmutableArray<IOperation> arguments, Compilation compilation)
    {
      var required = criteria.Count(p => !p.HasExplicitDefaultValue && !p.IsParams);
      var hasParams = criteria.Count > 0 && criteria[criteria.Count - 1].IsParams;
      if (arguments.Length < required || (!hasParams && arguments.Length > criteria.Count))
      {
        return false;
      }

      for (var i = 0; i < arguments.Length; i++)
      {
        var parameter = criteria[Math.Min(i, criteria.Count - 1)];
        var argument = arguments[i] is IConversionOperation { IsImplicit: true } conversion ? conversion.Operand : arguments[i];
        if (parameter.IsParams && parameter.Type is IArrayTypeSymbol paramsArray)
        {
          var isLastExpandable = i >= criteria.Count - 1;
          if (isLastExpandable && (IsConvertible(argument, paramsArray.ElementType, compilation) ||
            (arguments.Length == criteria.Count && IsConvertible(argument, paramsArray, compilation))))
          {
            continue;
          }

          return false;
        }

        if (!IsConvertible(argument, parameter.Type, compilation))
        {
          return false;
        }
      }

      return true;
    }

    private static bool IsConvertible(IOperation argument, ITypeSymbol target, Compilation compilation)
    {
      if (argument.Type is null)
      {
        // null literal or a typeless expression
        return argument.ConstantValue is not { HasValue: true, Value: null } ||
          target.IsReferenceType || target.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      }

      return compilation.ClassifyCommonConversion(argument.Type, target).IsImplicit;
    }

    /// <summary>
    /// The criteria values passed to the data portal method, or null when
    /// they are passed as an existing array.
    /// </summary>
    private static ImmutableArray<IOperation>? GetCriteriaArguments(IInvocationOperation invocation)
    {
      if (invocation.Arguments.Length == 0)
      {
        return ImmutableArray<IOperation>.Empty;
      }

      var argument = invocation.Arguments[invocation.Arguments.Length - 1];
      if (argument.ArgumentKind == ArgumentKind.ParamArray && argument.Value is IArrayCreationOperation { Initializer: { } initializer })
      {
        return initializer.ElementValues;
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

    /// <summary>
    /// Mirrors the generator: synchronous extension methods are generated unless
    /// the CslaGenerateSyncDataPortalExtensions MSBuild property is false, or
    /// there is no async suffix so they would have the same names as the async methods.
    /// </summary>
    private static bool GenerateSync(AnalyzerConfigOptions options)
    {
      return GetAsyncSuffix(options).Length > 0 &&
        !(options.TryGetValue("build_property.CslaGenerateSyncDataPortalExtensions", out var value) &&
        string.Equals(value?.Trim(), "false", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Mirrors the generator: async extension method names end with the
    /// CslaDataPortalExtensionsAsyncSuffix MSBuild property, which defaults
    /// to Async, is none for no suffix, and is ignored when it is not valid in an identifier.
    /// </summary>
    private static string GetAsyncSuffix(AnalyzerConfigOptions options)
    {
      if (options.TryGetValue("build_property.CslaDataPortalExtensionsAsyncSuffix", out var value) && value?.Trim() is { Length: > 0 } suffix)
      {
        if (string.Equals(suffix, "none", StringComparison.OrdinalIgnoreCase))
        {
          return string.Empty;
        }

        if (SyntaxFacts.IsValidIdentifier("_" + suffix))
        {
          return suffix;
        }
      }

      return "Async";
    }

    private sealed class KnownSymbols(
      string asyncSuffix,
      bool generateSync,
      INamedTypeSymbol dataPortalExtensionsAttribute,
      INamedTypeSymbol? iDataPortal,
      INamedTypeSymbol? iChildDataPortal,
      INamedTypeSymbol? dataPortal,
      INamedTypeSymbol? noDataPortalExtensionAttribute,
      INamedTypeSymbol? injectAttribute,
      INamedTypeSymbol? iCslaObject)
    {
      public string AsyncSuffix { get; } = asyncSuffix;
      public bool GenerateSync { get; } = generateSync;
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
