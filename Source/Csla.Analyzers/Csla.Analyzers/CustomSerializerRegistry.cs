using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// The custom serializers registered with <c>[assembly: MobileSerializerRegistration]</c>
  /// in a compilation and the assemblies it references. The CSLA source generator emits
  /// these attributes for <c>[MobileSerializer]</c> and <c>[MobileSerializerFor]</c>
  /// declarations, so they cover the declarations of the compilation itself, too.
  /// </summary>
  internal sealed class CustomSerializerRegistry
  {
    private static readonly IReadOnlyList<ITypeSymbol> None = [];

    // Shared by the analyzers that need it, so the referenced assemblies'
    // attributes are read once per compilation.
    private static readonly ConditionalWeakTable<Compilation, CustomSerializerRegistry> Cache = new();

    private readonly Dictionary<ITypeSymbol, List<ITypeSymbol>> _serializers;

    private CustomSerializerRegistry(Dictionary<ITypeSymbol, List<ITypeSymbol>> serializers)
    {
      _serializers = serializers;
    }

    /// <summary>
    /// Gets the registrations of a compilation and its referenced assemblies,
    /// reading them on first use.
    /// </summary>
    public static CustomSerializerRegistry Get(Compilation compilation, MobileSerializerSymbols? symbols, CancellationToken cancellationToken)
    {
      if (Cache.TryGetValue(compilation, out var registry))
        return registry;

      // Not created inside GetValue so a cancelled read isn't cached; a
      // registry built concurrently by another analyzer is simply discarded.
      registry = Create(compilation, symbols?.RegistrationAttribute, cancellationToken);
      return Cache.GetValue(compilation, _ => registry);
    }

    private static CustomSerializerRegistry Create(Compilation compilation, INamedTypeSymbol? registrationAttribute, CancellationToken cancellationToken)
    {
      var serializers = new Dictionary<ITypeSymbol, List<ITypeSymbol>>(SymbolEqualityComparer.Default);
      if (registrationAttribute is null)
        return new CustomSerializerRegistry(serializers);

      Add(compilation.Assembly);
      foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
      {
        cancellationToken.ThrowIfCancellationRequested();
        Add(assembly);
      }
      return new CustomSerializerRegistry(serializers);

      void Add(IAssemblySymbol assembly)
      {
        foreach (var attribute in assembly.GetAttributes())
        {
          if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, registrationAttribute))
            continue;
          if (!MobileSerializerRules.TryGetRegistration(attribute, out var type, out var serializer))
            continue;
          if (!serializers.TryGetValue(type, out var list))
          {
            list = [];
            serializers.Add(type, list);
          }
          if (!list.Contains(serializer, SymbolEqualityComparer.Default))
            list.Add(serializer);
        }
      }
    }

    /// <summary>
    /// Determines whether a custom serializer is registered for exactly this type.
    /// </summary>
    public bool IsRegistered(ITypeSymbol type) => _serializers.ContainsKey(type);

    /// <summary>
    /// Gets the serializers registered for exactly this type.
    /// </summary>
    public IReadOnlyList<ITypeSymbol> GetSerializers(ITypeSymbol type)
      => _serializers.TryGetValue(type, out var list) ? list : None;
  }
}
