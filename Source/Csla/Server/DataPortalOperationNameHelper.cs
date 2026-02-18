//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationNameHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Computes deterministic operation names for data portal dispatch</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Computes deterministic operation names for name-based data portal dispatch.
  /// The algorithm must agree with the source generator's naming logic.
  /// </summary>
  internal static class DataPortalOperationNameHelper
  {
    /// <summary>
    /// Computes the operation name from a resolved method and its operation attribute type.
    /// </summary>
    internal static string ComputeOperationName<T>(System.Reflection.MethodInfo methodInfo)
      where T : DataPortalOperationAttribute
    {
      var baseName = GetOperationBaseName<T>();
      var parameters = methodInfo.GetParameters();
      var criteriaTypeKeys = parameters
        .Where(p => !p.GetCustomAttributes(typeof(InjectAttribute), false).Any())
        .Select(p => GetTypeKey(p.ParameterType))
        .ToArray();

      if (criteriaTypeKeys.Length == 0)
        return baseName;

      return baseName + "__" + string.Join("_", criteriaTypeKeys);
    }

    /// <summary>
    /// Gets the base operation name from an attribute type.
    /// Strips the "Attribute" suffix (e.g. typeof(FetchAttribute) -> "Fetch").
    /// </summary>
    private static string GetOperationBaseName<T>()
      where T : DataPortalOperationAttribute
    {
      var name = typeof(T).Name;
      if (name.EndsWith("Attribute"))
        name = name[..^"Attribute".Length];
      return name;
    }

    /// <summary>
    /// Computes a deterministic type key from a runtime Type.
    /// Must produce the same result as the generator's GetOperationTypeKey(ITypeSymbol).
    /// </summary>
    internal static string GetTypeKey(Type type)
    {
      if (type.IsArray)
        return GetTypeKey(type.GetElementType()!) + "Array";

      if (type.IsGenericType)
      {
        // Type.Name for generics looks like "List`1" - same as MetadataName
        var baseName = type.Name.Replace('`', '_');
        var typeArgs = string.Join("_", type.GetGenericArguments().Select(GetTypeKey));
        return baseName + "_" + typeArgs;
      }

      return type.Name;
    }
  }
}
