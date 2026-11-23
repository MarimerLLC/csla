using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a data portal operation that completes an object (<c>[Create]</c>,
  /// <c>[Fetch]</c>, <c>[CreateChild]</c>, <c>[FetchChild]</c>, or <c>[Execute]</c>
  /// on a command) and doesn't visibly load a not-null reference-type property
  /// without a default value, declared in the type or a base type in source.
  /// Also reports a <c>BusinessBase</c> child type with such properties and
  /// child operations but no <c>[CreateChild]</c> of its own.
  /// </summary>
  /// <remarks>
  /// Best-effort, and silent when in doubt: the data portal check is the safety net.
  /// A load is a call to <c>LoadProperty</c>, <c>LoadPropertyMarkDirty</c>,
  /// <c>LoadPropertyConvert</c>, <c>SetProperty</c> or <c>SetPropertyConvert</c>
  /// with the property's field on this object, or an assignment to the property,
  /// anywhere in the operation (no path analysis); a call on a newly created object
  /// loads nothing. An operation that passes <c>this</c> to a method, calls a method
  /// of its own type hierarchy declared in source, loads through a receiver that
  /// isn't this object or a new one, or loads through a property info that isn't a
  /// field, isn't reported. Types with <c>[ObjectFactory]</c> aren't analyzed.
  /// A command's <c>[Create]</c> isn't checked by the data portal; a property its
  /// <c>[Create]</c> loads, or that has a non-private setter the caller can use
  /// before Execute, counts as loaded, and a command that inherits a <c>[Create]</c>
  /// isn't reported.
  /// </remarks>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class NotNullPropertyNotLoadedAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly ImmutableHashSet<string> loadMethods =
    [
      CslaMemberConstants.Properties.LoadProperty,
      CslaMemberConstants.Properties.LoadPropertyMarkDirty,
      CslaMemberConstants.Properties.LoadPropertyConvert,
      CslaMemberConstants.Properties.SetProperty,
      CslaMemberConstants.Properties.SetPropertyConvert
    ];

    private static readonly ImmutableHashSet<string> completingOperationAttributes =
    [
      CslaMemberConstants.Types.CreateAttribute,
      CslaMemberConstants.Types.FetchAttribute,
      CslaMemberConstants.Types.CreateChildAttribute,
      CslaMemberConstants.Types.FetchChildAttribute
    ];

    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.NotNullPropertyNotLoaded, NotNullPropertyNotLoadedAnalyzerConstants.Title,
        NotNullPropertyNotLoadedAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.NotNullPropertyNotLoaded, nameof(NotNullPropertyNotLoadedAnalyzer)));

    private static readonly DiagnosticDescriptor missingCreateChildRule =
      new(
        Constants.AnalyzerIdentifiers.NotNullPropertyNotLoaded, NotNullPropertyNotLoadedAnalyzerConstants.Title,
        NotNullPropertyNotLoadedAnalyzerConstants.MissingCreateChildMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.NotNullPropertyNotLoaded, nameof(NotNullPropertyNotLoadedAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule, missingCreateChildRule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSymbolStartAction(AnalyzeTypeStart, SymbolKind.NamedType);
    }

    private static void AnalyzeTypeStart(SymbolStartAnalysisContext context)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      if (type.TypeKind != TypeKind.Class || type.HasObjectFactoryAttribute())
      {
        return;
      }

      var checkedFields = GetCheckedFields(type, context.CancellationToken);
      if (checkedFields.Count == 0)
      {
        return;
      }

      var isCommand = type.IsCommandBase();
      if (isCommand && InheritsCommandCreate(type))
      {
        // a base type's [Create] isn't in this type's operation blocks, so
        // what it loads for [Execute] can't be read
        return;
      }

      var operations = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
      var commandCreates = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
      foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
      {
        if (isCommand)
        {
          if (HasAttribute(method, CslaMemberConstants.Types.ExecuteAttribute))
            operations.Add(method);
          else if (HasAttribute(method, CslaMemberConstants.Types.CreateAttribute))
            commandCreates.Add(method);
        }
        else if (method.HasOperationAttribute(_ => _ is not null && completingOperationAttributes.Contains(_.Name) && _.IsDeclaredInCsla()))
        {
          operations.Add(method);
        }
      }

      var lacksOwnCreateChild = !type.IsAbstract && type.IsBusinessBase() &&
        type.GetMembers().OfType<IMethodSymbol>().Any(_ => _.HasOperationAttribute(a => a.IsDataPortalChildOperationAttribute())) &&
        !HasSourceCreateChild(type);

      if (operations.Count == 0 && !lacksOwnCreateChild)
      {
        return;
      }

      var states = new ConcurrentDictionary<IMethodSymbol, OperationState>(SymbolEqualityComparer.Default);
      context.RegisterOperationBlockStartAction(blockStart =>
      {
        if (blockStart.OwningSymbol is not IMethodSymbol method ||
          !(operations.Contains(method) || commandCreates.Contains(method)))
        {
          return;
        }

        var state = states.GetOrAdd(method, _ => new OperationState());
        if (OnlyThrows(blockStart.OperationBlocks))
        {
          // never completes an object, so it isn't checked; a command's
          // throwing [Create] simply loads nothing for [Execute]
          if (operations.Contains(method))
            state.Silence();
          return;
        }
        blockStart.RegisterOperationAction(c => AnalyzeInvocation(c, state, type, checkedFields), OperationKind.Invocation);
        blockStart.RegisterOperationAction(c => AnalyzeObjectCreation(c, state), OperationKind.ObjectCreation);
        blockStart.RegisterOperationAction(c => AnalyzeAssignment(c, state, type),
          OperationKind.SimpleAssignment, OperationKind.CompoundAssignment, OperationKind.CoalesceAssignment,
          OperationKind.DeconstructionAssignment);
      });

      context.RegisterSymbolEndAction(end =>
      {
        var preloaded = new HashSet<string>(StringComparer.Ordinal);
        if (isCommand)
        {
          foreach (var create in commandCreates)
          {
            if (!states.TryGetValue(create, out var createState) || createState.IsSilent)
            {
              // a [Create] we can't read could load anything
              if (createState is not null)
                return;
              continue;
            }
            preloaded.UnionWith(createState.Loaded);
          }
          foreach (var name in checkedFields.Values)
          {
            if (HasSettableProperty(type, name))
              preloaded.Add(name);
          }
        }

        foreach (var operation in operations)
        {
          if (!states.TryGetValue(operation, out var state) || state.IsSilent)
          {
            continue;
          }
          var loaded = state.Loaded;
          var missing = checkedFields.Values
            .Where(_ => !loaded.Contains(_) && !preloaded.Contains(_))
            .Distinct()
            .ToList();
          if (missing.Count > 0)
          {
            end.ReportDiagnostic(Diagnostic.Create(rule, operation.Locations[0], operation.Name, string.Join(", ", missing)));
          }
        }

        if (lacksOwnCreateChild)
        {
          end.ReportDiagnostic(Diagnostic.Create(missingCreateChildRule, GetUserLocation(type, end.CancellationToken), type.Name,
            string.Join(", ", checkedFields.Values.Distinct())));
        }
      });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, OperationState state,
      INamedTypeSymbol type, Dictionary<IFieldSymbol, string> checkedFields)
    {
      var invocation = (IInvocationOperation)context.Operation;
      var method = invocation.TargetMethod;

      if (loadMethods.Contains(method.Name) && method.ContainingType.IsDeclaredInCsla())
      {
        switch (NotNullPropertyRegistrations.Unwrap(invocation.Instance))
        {
          case null:
          case IInstanceReferenceOperation { ReferenceKind: InstanceReferenceKind.ContainingTypeInstance }:
            break;
          case IObjectCreationOperation:
            return; // loads a new object, not the one this operation completes
          default:
            state.Silence(); // a receiver we can't follow could be this object
            return;
        }
        foreach (var argument in invocation.Arguments)
        {
          if (argument.Parameter?.Type.IsIPropertyInfo() != true)
            continue;
          var field = NotNullPropertyRegistrations.GetReferencedField(argument.Value);
          if (field is null)
            state.Silence(); // a property info we can't follow could be any property
          else if (checkedFields.TryGetValue(field.OriginalDefinition, out var name))
            state.Load(name);
        }
        return;
      }

      if (PassesThis(invocation.Arguments))
      {
        state.Silence();
        return;
      }

      var target = method.ReducedFrom ?? method;
      if (target.MethodKind is not (MethodKind.LocalFunction or MethodKind.AnonymousFunction) &&
        IsInHierarchy(target.ContainingType, type) &&
        target.Locations.Any(_ => _.IsInSource))
      {
        state.Silence(); // a helper of the type's own hierarchy may load the properties
      }
    }

    private static void AnalyzeObjectCreation(OperationAnalysisContext context, OperationState state)
    {
      if (PassesThis(((IObjectCreationOperation)context.Operation).Arguments))
      {
        state.Silence();
      }
    }

    private static void AnalyzeAssignment(OperationAnalysisContext context, OperationState state, INamedTypeSymbol type) =>
      LoadAssignedProperties(((IAssignmentOperation)context.Operation).Target, state, type);

    /// <summary>
    /// Records an assignment target that is a property of this object, including
    /// each element of a deconstruction target such as <c>(Item, Name) = ...</c>.
    /// </summary>
    private static void LoadAssignedProperties(IOperation target, OperationState state, INamedTypeSymbol type)
    {
      switch (target)
      {
        case ITupleOperation tuple:
          foreach (var element in tuple.Elements)
          {
            LoadAssignedProperties(element, state, type);
          }
          break;
        case IPropertyReferenceOperation { Instance: null or IInstanceReferenceOperation { ReferenceKind: InstanceReferenceKind.ContainingTypeInstance } } propertyReference
          when IsInHierarchy(propertyReference.Property.ContainingType, type):
          state.Load(propertyReference.Property.Name);
          break;
      }
    }

    /// <summary>
    /// Gets whether an operation's body only throws (<c>=&gt; throw ...</c> or
    /// <c>{ throw ...; }</c>): it never completes an object, so the data portal
    /// never checks it.
    /// </summary>
    private static bool OnlyThrows(ImmutableArray<IOperation> operationBlocks)
    {
      foreach (var block in operationBlocks)
      {
        if (block is not IBlockOperation { Operations.Length: 1 } body)
        {
          continue;
        }
        var statement = body.Operations[0] switch
        {
          IExpressionStatementOperation expression => expression.Operation,
          IReturnOperation { ReturnedValue: { } returned } => NotNullPropertyRegistrations.Unwrap(returned),
          var other => other
        };
        return statement is IThrowOperation;
      }
      return false;
    }

    /// <summary>
    /// Gets a declaration of a partial type that isn't generated code, where a
    /// diagnostic is shown; the first declaration can be source generator output.
    /// </summary>
    private static Location GetUserLocation(INamedTypeSymbol type, CancellationToken cancellationToken) =>
      type.Locations.FirstOrDefault(_ => _.SourceTree is { } tree && !IsGenerated(tree, cancellationToken)) ?? type.Locations[0];

    private static bool IsGenerated(SyntaxTree tree, CancellationToken cancellationToken) =>
      tree.FilePath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
      tree.FilePath.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase) ||
      tree.GetRoot(cancellationToken).GetLeadingTrivia().ToString().Contains("<auto-generated");

    private static bool PassesThis(ImmutableArray<IArgumentOperation> arguments) =>
      arguments.Any(_ => NotNullPropertyRegistrations.Unwrap(_.Value) is IInstanceReferenceOperation
      {
        ReferenceKind: InstanceReferenceKind.ContainingTypeInstance
      });

    private static bool HasAttribute(IMethodSymbol method, string attributeName) =>
      method.HasOperationAttribute(_ => _ is not null && _.Name == attributeName && _.IsDeclaredInCsla());

    /// <summary>
    /// Gets the checked fields of a type and its base types declared in source,
    /// keyed by their original definition, with the names they register.
    /// The members of the constructed base types are read, so a generic base's
    /// <c>NotNullPropertyInfo&lt;P&gt;</c> is checked with its type argument.
    /// </summary>
    private static Dictionary<IFieldSymbol, string> GetCheckedFields(INamedTypeSymbol type, CancellationToken cancellationToken)
    {
      var result = new Dictionary<IFieldSymbol, string>(SymbolEqualityComparer.Default);
      for (var current = type; current is not null && !current.IsDeclaredInCsla(); current = current.BaseType)
      {
        foreach (var member in current.GetMembers())
        {
          if (member is IFieldSymbol { IsStatic: true } field &&
            NotNullPropertyRegistrations.IsNoDefaultReference(field, cancellationToken))
          {
            result[field.OriginalDefinition] = NotNullPropertyRegistrations.GetRegisteredName(field, cancellationToken);
          }
        }
      }
      return result;
    }

    private static bool InheritsCommandCreate(INamedTypeSymbol type)
    {
      for (var current = type.BaseType; current is not null && !current.IsDeclaredInCsla(); current = current.BaseType)
      {
        if (current.GetMembers().OfType<IMethodSymbol>().Any(_ => HasAttribute(_, CslaMemberConstants.Types.CreateAttribute)))
        {
          return true;
        }
      }
      return false;
    }

    private static bool HasSourceCreateChild(INamedTypeSymbol type)
    {
      for (var current = type; current is not null && !current.IsDeclaredInCsla(); current = current.BaseType)
      {
        if (current.GetMembers().OfType<IMethodSymbol>().Any(_ => HasAttribute(_, CslaMemberConstants.Types.CreateChildAttribute)))
        {
          return true;
        }
      }
      return false;
    }

    private static bool HasSettableProperty(INamedTypeSymbol type, string name)
    {
      for (var current = type; current is not null && !current.IsDeclaredInCsla(); current = current.BaseType)
      {
        foreach (var member in current.GetMembers(name))
        {
          if (member is IPropertySymbol { SetMethod: { } setter } && setter.DeclaredAccessibility != Accessibility.Private)
          {
            return true;
          }
        }
      }
      return false;
    }

    private static bool IsInHierarchy(INamedTypeSymbol? candidate, INamedTypeSymbol type)
    {
      if (candidate is null)
      {
        return false;
      }
      for (var current = type; current is not null; current = current.BaseType)
      {
        if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, candidate.OriginalDefinition))
        {
          return true;
        }
      }
      return false;
    }

    private sealed class OperationState
    {
      private readonly object _lock = new();
      private readonly HashSet<string> _loaded = new(StringComparer.Ordinal);
      private bool _silent;

      public bool IsSilent
      {
        get
        {
          lock (_lock)
          {
            return _silent;
          }
        }
      }

      /// <summary>
      /// Gets a copy of the loaded property names; read it once per report.
      /// </summary>
      public HashSet<string> Loaded
      {
        get
        {
          lock (_lock)
          {
            return new HashSet<string>(_loaded, StringComparer.Ordinal);
          }
        }
      }

      public void Load(string name)
      {
        lock (_lock)
        {
          _loaded.Add(name);
        }
      }

      public void Silence()
      {
        lock (_lock)
        {
          _silent = true;
        }
      }
    }
  }
}
