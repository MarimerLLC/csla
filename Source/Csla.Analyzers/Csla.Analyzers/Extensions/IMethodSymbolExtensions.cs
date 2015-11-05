using Microsoft.CodeAnalysis;

namespace Csla.Analyzers.Extensions
{
  internal static class IMethodSymbolExtensions
  {
    internal static bool IsPropertyInfoManagementMethod(this IMethodSymbol @this)
    {
      return @this != null && (@this.ContainingType.Name == CslaMemberConstants.CslaTypeNames.BusinessBase &&
        (@this.Name == CslaMemberConstants.CslaPropertyMethods.GetProperty ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.GetPropertyConvert ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.ReadProperty ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.ReadPropertyConvert ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LazyGetProperty ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LazyGetPropertyAsync ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LazyReadProperty ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LazyReadPropertyAsync ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LoadProperty ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LoadPropertyAsync ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.LoadPropertyConvert ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.SetProperty ||
        @this.Name == CslaMemberConstants.CslaPropertyMethods.SetPropertyConvert));
    }

    internal static bool IsDataPortalOperation(this IMethodSymbol @this)
    {
      return @this != null && (@this.Name == CslaMemberConstants.CslaOperations.DataPortalCreate || 
        @this.Name == CslaMemberConstants.CslaOperations.DataPortalFetch ||
        @this.Name == CslaMemberConstants.CslaOperations.DataPortalInsert || 
        @this.Name == CslaMemberConstants.CslaOperations.DataPortalUpdate ||
        @this.Name == CslaMemberConstants.CslaOperations.DataPortalDelete || 
        @this.Name == CslaMemberConstants.CslaOperations.DataPortalDeleteSelf ||
        @this.Name == CslaMemberConstants.CslaOperations.DataPortalExecute ||
        @this.Name == CslaMemberConstants.CslaOperations.ChildCreate || 
        @this.Name == CslaMemberConstants.CslaOperations.ChildFetch ||
        @this.Name == CslaMemberConstants.CslaOperations.ChildInsert || 
        @this.Name == CslaMemberConstants.CslaOperations.ChildUpdate ||
        @this.Name == CslaMemberConstants.CslaOperations.ChildDeleteSelf);
    }
  }
}
