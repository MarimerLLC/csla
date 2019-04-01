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
    internal static bool IsObjectFactory(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.ObjectFactoryBase &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsObjectFactory());
    }

    internal static bool IsBusinessBase(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.BusinessBase &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsBusinessBase());
    }

    internal static bool IsPrimitive(this ITypeSymbol @this)
    {
      var specialType = @this.SpecialType;
      return specialType == SpecialType.System_Boolean ||
        specialType == SpecialType.System_Char ||
        specialType == SpecialType.System_String ||
        specialType == SpecialType.System_Byte ||
        specialType == SpecialType.System_SByte ||
        specialType == SpecialType.System_Int16 ||
        specialType == SpecialType.System_UInt16 ||
        specialType == SpecialType.System_Int32 ||
        specialType == SpecialType.System_UInt32 ||
        specialType == SpecialType.System_Int64 ||
        specialType == SpecialType.System_UInt64 ||
        specialType == SpecialType.System_Single ||
        specialType == SpecialType.System_Double;
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