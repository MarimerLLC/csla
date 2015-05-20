using Microsoft.CodeAnalysis;

namespace Csla.Analyzers.Extensions
{
	internal static class IMethodSymbolExtensions
	{
		internal static bool IsDataPortalOperation(this IMethodSymbol @this)
		{
			if(@this == null)
			{
				return false;
			}
			else
			{
				return @this.Name == "DataPortal_Create" || @this.Name == "DataPortal_Fetch" ||
					@this.Name == "DataPortal_Insert" || @this.Name == "DataPortal_Update" ||
					@this.Name == "DataPortal_Delete" || @this.Name == "DataPortal_DeleteSelf" ||
					@this.Name == "DataPortal_Execute" ||
					@this.Name == "Child_Create" || @this.Name == "Child_Fetch" ||
					@this.Name == "Child_Insert" || @this.Name == "Child_Update" ||
					@this.Name == "Child_DeleteSelf"; 
			}
		}
	}
}
