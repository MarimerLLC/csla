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

    private readonly Dictionary<ITypeSymbol, List<ITypeSymbol>> _serializers;

    private CustomSerializerRegistry(Dictionary<ITypeSymbol, List<ITypeSymbol>> serializers)
    {
      _serializers = serializers;
    }

    /// <summary>
    /// Reads the registrations of a compilation and its referenced assemblies.
    /// </summary>
    public static CustomSerializerRegistry Create(Compilation compilation, CancellationToken cancellationToken = default)
    {
      var serializers = new Dictionary<ITypeSymbol, List<ITypeSymbol>>(SymbolEqualityComparer.Default);
      var registrationAttribute = compilation.GetTypeByMetadataName(MobileSerializerRules.MobileSerializerRegistrationAttributeMetadataName);
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
