using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using static System.Reflection.IntrospectionExtensions;

namespace Csla.Analyzers.Extensions
{
  internal static class ITypeSymbolExtensions
  {
    internal static bool IsBusinessBase(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.BusinessBase &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsBusinessBase());
    }

    internal static bool IsIPropertyInfo(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.IPropertyInfo &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsIPropertyInfo() || @this.Interfaces.Any(_ => _.IsIPropertyInfo()));
    }

    internal static bool IsEditableStereotype(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.Types.BusinessBase ||
          @this.Name == CslaMemberConstants.Types.BusinessListBase ||
          @this.Name == CslaMemberConstants.Types.DynamicListBase ||
          @this.Name == CslaMemberConstants.Types.BusinessBindingListBase) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsEditableStereotype());
    }

    internal static bool IsSerializable(this ITypeSymbol @this)
    {
      // Either the symbol is a delegate or enum
      // or it has the [SerializableAttribute]
      return @this != null && (
        @this.TypeKind == TypeKind.Enum || @this.TypeKind == TypeKind.Delegate ||
        @this.GetAttributes().Any(
          _ => _.AttributeClass.Name == CslaMemberConstants.SerializableAttribute) ||
        @this.HasSerializableFlag());
    }

    private static bool HasSerializableFlag(this ITypeSymbol @this)
    {
      var flagsProperty = @this.GetAllProperties()
        .SingleOrDefault(_ => _.Name == "Flags" && _.PropertyType == typeof(TypeAttributes) &&
          _.CanRead);

      return flagsProperty != null && 
        ((TypeAttributes)flagsProperty?.GetValue(@this)).HasFlag(TypeAttributes.Serializable);
    }

    private static ImmutableArray<PropertyInfo> GetAllProperties(this ITypeSymbol @this)
    {
      var properties = new List<PropertyInfo>();

      var type = @this.GetType().GetTypeInfo();

      while(type != null)
      {
        properties.AddRange(type.DeclaredProperties);
        type = type.BaseType?.GetTypeInfo();
      }

      return properties.ToImmutableArray();
    }

    internal static bool IsStereotype(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.Types.IBusinessObject ||
          @this.Name == CslaMemberConstants.Types.DynamicListBase) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsStereotype() || @this.Interfaces.Any(_ => _.IsStereotype())));
    }

    internal static bool IsMobileObject(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.IMobileObject &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsMobileObject() || @this.Interfaces.Any(_ => _.IsMobileObject())));
    }
  }
}