using System.Linq;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers.Extensions
{
  internal static class IMethodSymbolExtensions
  {
    internal static bool IsPropertyInfoManagementMethod(this IMethodSymbol @this)
    {
      return @this != null &&
        ((@this.ContainingType.Name == CslaMemberConstants.Types.BusinessBase &&
          (@this.Name == CslaMemberConstants.Properties.SetProperty ||
          @this.Name == CslaMemberConstants.Properties.SetPropertyConvert)) ||
        ((@this.ContainingType.Name == CslaMemberConstants.Types.BusinessBase ||
          @this.ContainingType.Name == CslaMemberConstants.Types.ReadOnlyBase) &&
            (@this.Name == CslaMemberConstants.Properties.GetProperty ||
            @this.Name == CslaMemberConstants.Properties.GetPropertyConvert ||
            @this.Name == CslaMemberConstants.Properties.LazyGetProperty ||
            @this.Name == CslaMemberConstants.Properties.LazyGetPropertyAsync ||
            @this.Name == CslaMemberConstants.Properties.LazyReadProperty ||
            @this.Name == CslaMemberConstants.Properties.LazyReadPropertyAsync ||
            @this.Name == CslaMemberConstants.Properties.LoadPropertyAsync)) ||
        ((@this.ContainingType.Name == CslaMemberConstants.Types.BusinessBase ||
          @this.ContainingType.Name == CslaMemberConstants.Types.ManagedObjectBase) &&
            @this.Name == CslaMemberConstants.Properties.LoadPropertyMarkDirty) ||
        ((@this.ContainingType.Name == CslaMemberConstants.Types.BusinessBase ||
          @this.ContainingType.Name == CslaMemberConstants.Types.ReadOnlyBase ||
          @this.ContainingType.Name == CslaMemberConstants.Types.ManagedObjectBase) &&
            (@this.Name == CslaMemberConstants.Properties.ReadProperty ||
            @this.Name == CslaMemberConstants.Properties.ReadPropertyConvert ||
            @this.Name == CslaMemberConstants.Properties.LoadProperty ||
            @this.Name == CslaMemberConstants.Properties.LoadPropertyConvert)));
    }

    internal static DataPortalOperationQualification IsDataPortalOperation(this IMethodSymbol @this)
    {
      return @this is null ? new DataPortalOperationQualification() :
        @this.IsRootDataPortalOperation().Combine(@this.IsChildDataPortalOperation());
    }

    internal static DataPortalOperationQualification IsRootDataPortalOperation(this IMethodSymbol @this)
    {
      if (@this is null)
      {
        return new DataPortalOperationQualification();
      }
      else
      {
        var byNamingConvention = 
          @this.Name == CslaMemberConstants.Operations.DataPortalCreate ||
          @this.Name == CslaMemberConstants.Operations.DataPortalFetch ||
          @this.Name == CslaMemberConstants.Operations.DataPortalInsert ||
          @this.Name == CslaMemberConstants.Operations.DataPortalUpdate ||
          @this.Name == CslaMemberConstants.Operations.DataPortalDelete ||
          @this.Name == CslaMemberConstants.Operations.DataPortalDeleteSelf ||
          @this.Name == CslaMemberConstants.Operations.DataPortalExecute;
        var byAttribute = @this.GetAttributes().Any(_ => _.AttributeClass.IsDataPortalRootOperationAttribute());
        return new DataPortalOperationQualification(byNamingConvention, byAttribute);
      }
    }

    internal static DataPortalOperationQualification IsChildDataPortalOperation(this IMethodSymbol @this)
    {
      if (@this is null)
      {
        return new DataPortalOperationQualification();
      }
      else
      {
        var byNamingConvention =
          @this.Name == CslaMemberConstants.Operations.ChildCreate ||
          @this.Name == CslaMemberConstants.Operations.ChildFetch ||
          @this.Name == CslaMemberConstants.Operations.ChildInsert ||
          @this.Name == CslaMemberConstants.Operations.ChildUpdate ||
          @this.Name == CslaMemberConstants.Operations.ChildDeleteSelf ||
          @this.Name == CslaMemberConstants.Operations.ChildExecute;
        var byAttribute = @this.GetAttributes().Any(_ => _.AttributeClass.IsDataPortalChildOperationAttribute());
        return new DataPortalOperationQualification(byNamingConvention, byAttribute);
      }
    }
  }
}
