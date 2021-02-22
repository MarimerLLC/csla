using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Csla.Analyzers.Extensions
{
  internal static class ITypeSymbolExtensions
  {
    private static readonly Type[] SerializableTypesByMobileFormatter =
      new[] { typeof(TimeSpan), typeof(DateTimeOffset), typeof(byte[]), typeof(byte[][]), typeof(char[]), typeof(Guid), typeof(List<int>) };

    internal static bool IsBusinessRule(this ITypeSymbol @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.Types.IBusinessRule || @this.Name == CslaMemberConstants.Types.IBusinessRuleAsync) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsBusinessRule() || @this.Interfaces.Any(_ => _.IsBusinessRule())));
    }

    internal static bool IsObjectFactory(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.ObjectFactory &&
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

    internal static bool IsInjectable(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.InjectAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsInjectable());
    }

    internal static bool IsDataPortalOperationAttribute(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.DataPortalOperationAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsDataPortalOperationAttribute());
    }

    internal static bool IsDataPortalRootOperationAttribute(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.DataPortalRootOperationAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsDataPortalRootOperationAttribute());
    }

    internal static bool IsDataPortalChildOperationAttribute(this ITypeSymbol @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.DataPortalChildOperationAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsDataPortalChildOperationAttribute());
    }

    internal static bool IsRunLocalAttribute(this ITypeSymbol @this)
    {
      return @this != null &&
        @this.Name == CslaMemberConstants.Types.RunLocalAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName;
    }

    internal static bool IsSerializableByMobileFormatter(this ITypeSymbol @this, Compilation compilation)
    {
      if(@this.TypeKind == TypeKind.Enum)
      {
        return true;
      }
      else
      {
        foreach(var serializableType in SerializableTypesByMobileFormatter)
        {
          var type = compilation.GetTypeByMetadataName(serializableType.FullName);

          if(@this.Equals(type))
          {
            return true;
          }
        }
      }

      return false;
    }

    internal static bool IsSpecialTypeSerializable(this ITypeSymbol @this)
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
        specialType == SpecialType.System_Double ||
        specialType == SpecialType.System_Decimal ||
        specialType == SpecialType.System_DateTime;
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