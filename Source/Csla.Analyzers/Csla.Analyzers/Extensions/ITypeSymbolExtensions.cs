using Microsoft.CodeAnalysis;
using System.Linq;

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
      // Either the symbol is a value type,
      // or it has the [SerializableAttribute]
      // or one of its interfaces implements ISerializable
      // (or one of the base interfaces implements ISerializable)
      return @this != null && (
        @this.IsValueType ||
        @this.GetAttributes().Any(
          _ => _.AttributeClass.Name == CslaMemberConstants.SerializableAttribute) ||
        @this.Interfaces.Any(
          _ => (_.Name == DotNetMemberConstants.Types.ISerializable && 
            _.GetFullNamespace() == DotNetMemberConstants.Namespaces.ISerializable) ||
            _.IsSerializable()));
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