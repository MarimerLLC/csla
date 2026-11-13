using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers.Extensions
{
  internal static class ITypeSymbolExtensions
  {
    private static readonly Type[] SerializableTypesByMobileFormatter =
      [typeof(TimeSpan), typeof(DateTimeOffset), typeof(byte[]), typeof(byte[][]), typeof(char[]), typeof(Guid), typeof(List<int>)];

    internal static bool IsBusinessRule([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.Types.IBusinessRule || @this.Name == CslaMemberConstants.Types.IBusinessRuleAsync) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsBusinessRule() || @this.Interfaces.Any(_ => _.IsBusinessRule())));
    }

    internal static bool IsObjectFactory([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.ObjectFactory &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsObjectFactory());
    }

    internal static bool IsBusinessBase([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.BusinessBase &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsBusinessBase());
    }

    internal static bool IsInjectable([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.InjectAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsInjectable());
    }

    internal static bool IsDataPortalOperationAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.DataPortalOperationAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsDataPortalOperationAttribute());
    }

    internal static bool IsDataPortalRootOperationAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.DataPortalRootOperationAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsDataPortalRootOperationAttribute());
    }

    internal static bool IsDataPortalChildOperationAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.DataPortalChildOperationAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsDataPortalChildOperationAttribute());
    }

    internal static bool IsRunLocalAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
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

#pragma warning disable RS1024 // Symbols should be compared for equality
          if (@this.Equals(type))
          {
            return true;
          }
#pragma warning restore RS1024 // Symbols should be compared for equality
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

    internal static bool IsIPropertyInfo([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.IPropertyInfo &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsIPropertyInfo() || @this.Interfaces.Any(_ => _.IsIPropertyInfo()));
    }

    internal static bool IsEditableStereotype([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.Types.BusinessBase ||
          @this.Name == CslaMemberConstants.Types.BusinessListBase ||
          @this.Name == CslaMemberConstants.Types.DynamicListBase ||
          @this.Name == CslaMemberConstants.Types.BusinessBindingListBase) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsEditableStereotype());
    }

    internal static bool IsStereotype([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        (((@this.Name == CslaMemberConstants.Types.IBusinessObject ||
          @this.Name == CslaMemberConstants.Types.DynamicListBase) &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsStereotype() || @this.Interfaces.Any(_ => _.IsStereotype())));
    }

    internal static bool IsMobileObject([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.IMobileObject &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          (@this.BaseType.IsMobileObject() || @this.Interfaces.Any(_ => _.IsMobileObject())));
    }

    internal static bool IsObjectAuthorizationRulesAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.ObjectAuthorizationRulesAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsObjectAuthorizationRulesAttribute());
    }

    /// <summary>
    /// Gets whether the type or one of its base types has <c>[ObjectFactory]</c>
    /// or an attribute derived from it, matching how the data portal looks for
    /// the attribute (<c>GetCustomAttributes(typeof(ObjectFactoryAttribute), true)</c>).
    /// </summary>
    internal static bool HasObjectFactoryAttribute(this ITypeSymbol? @this)
    {
      for (var type = @this; type is not null; type = type.BaseType)
      {
        if (type.GetAttributes().Any(_ => _.AttributeClass.IsObjectFactoryAttribute()))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Gets whether the type implements <c>ICslaObject</c>, the constraint the
    /// data portal places on the types it creates, fetches and updates.
    /// </summary>
    internal static bool IsCslaObject([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        @this.AllInterfaces.Any(_ =>
          _.Name == CslaMemberConstants.Types.ICslaObject &&
          _.ContainingAssembly.Name == CslaMemberConstants.AssemblyName);
    }

    internal static bool IsObjectFactoryAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        ((@this.Name == CslaMemberConstants.Types.ObjectFactoryAttribute &&
          @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName) ||
          @this.BaseType.IsObjectFactoryAttribute());
    }

    internal static bool IsCalculatedPropertyAttribute([NotNullWhen(true)] this ITypeSymbol? @this)
    {
      return @this != null &&
        @this.Name == CslaMemberConstants.Types.CalculatedPropertyAttribute &&
        @this.ContainingAssembly.Name == CslaMemberConstants.AssemblyName;
    }
  }
}