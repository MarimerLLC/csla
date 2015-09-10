using Microsoft.CodeAnalysis;
using System.Linq;

namespace Csla.Analyzers.Extensions
{
  internal static class ITypeSymbolExtensions
  {
    internal static bool IsBusinessBase(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == "BusinessBase" && @this.ContainingAssembly.Name == "Csla") ||
          @this.BaseType.IsBusinessBase());
    }

    internal static bool IsEditableStereotype(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == "BusinessBase" || @this.Name == "BusinessListBase" || 
          @this.Name == "DynamicListBase" || @this.Name == "BusinessBindingListBase") && 
          @this.ContainingAssembly.Name == "Csla") ||
          @this.BaseType.IsEditableStereotype());
    }

    internal static bool IsSerializable(this ITypeSymbol @this)
    {
      return @this != null && 
        @this.GetAttributes().Any(_ => _.AttributeClass.Name == "SerializableAttribute");
    }

    internal static bool IsStereotype(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == "IBusinessObject" || @this.Name == "DynamicListBase") && 
          @this.ContainingAssembly.Name == "Csla") ||
          (@this.BaseType.IsStereotype() || @this.Interfaces.Any(_ => _.IsStereotype())));
    }
  }
}