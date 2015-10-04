using Microsoft.CodeAnalysis;
using System.Linq;

namespace Csla.Analyzers.Extensions
{
  internal static class ITypeSymbolExtensions
  {
    internal static bool IsBusinessBase(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.CslaTypeNames.BusinessBase &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsBusinessBase());
    }

    internal static bool IsEditableStereotype(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.CslaTypeNames.BusinessBase ||
          @this.Name == CslaMemberConstants.CslaTypeNames.BusinessListBase ||
          @this.Name == CslaMemberConstants.CslaTypeNames.DynamicListBase ||
          @this.Name == CslaMemberConstants.CslaTypeNames.BusinessBindingListBase) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsEditableStereotype());
    }

    internal static bool IsSerializable(this ITypeSymbol @this)
    {
      return @this != null &&
        @this.GetAttributes().Any(_ => _.AttributeClass.Name == CslaMemberConstants.SerializableAttribute);
    }

    internal static bool IsStereotype(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.CslaTypeNames.IBusinessObject ||
          @this.Name == CslaMemberConstants.CslaTypeNames.DynamicListBase) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsStereotype() || @this.Interfaces.Any(_ => _.IsStereotype())));
    }
  }
}