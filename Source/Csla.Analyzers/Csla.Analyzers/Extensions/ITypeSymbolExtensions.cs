using Microsoft.CodeAnalysis;
using System.Linq;

namespace Csla.Analyzers.Extensions
{
	internal static class ITypeSymbolExtensions
	{
		internal static bool IsSerializable(this ITypeSymbol @this)
		{
			if(@this == null)
			{
				return false;
			}
			
			foreach(var attributeData in @this.GetAttributes())
			{
				var attributeClass = attributeData.AttributeClass;

            if (attributeClass.Name == "SerializableAttribute")
				{
					return true;
				}
         }

			return false;
		}

		internal static bool IsStereotype(this ITypeSymbol @this)
		{
			if(@this == null)
			{
				return false;
			}
			else
			{
				if (@this.Name == "IBusinessObject" &&
					@this.ContainingAssembly.Name == "Csla")
				{
					return true;
				}
				else
				{
					return @this.BaseType.IsStereotype() || 
						@this.Interfaces.Where(_ => _.IsStereotype()).Any();
				}
			}
		}
	}
}
