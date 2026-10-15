//-----------------------------------------------------------------------
// <copyright file="PropertyLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Loads property values through a non-generic IPropertyInfo</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Csla.Core
{
  /// <summary>
  /// Implemented by <see cref="PropertyInfo{T}"/> so a value can be loaded
  /// through a non-generic <see cref="IPropertyInfo"/> by calling the strongly
  /// typed <see cref="IManageProperties"/> methods, without creating generic
  /// method instantiations at runtime.
  /// </summary>
  internal interface ITypedPropertyLoader
  {
    bool TryLoadProperty(IManageProperties target, object? newValue);
    bool TryLoadPropertyMarkDirty(IManageProperties target, object? newValue, out bool valuesDiffer);
  }

  /// <summary>
  /// Loads property values when only an <see cref="IPropertyInfo"/> is available.
  /// </summary>
  internal static class PropertyLoader
  {
    public static void LoadProperty(IManageProperties target, IPropertyInfo propertyInfo, object? newValue)
    {
      if (propertyInfo is ITypedPropertyLoader loader && loader.TryLoadProperty(target, newValue))
        return;

      InvokeGenericLoadMethod(target, "LoadProperty", propertyInfo, newValue);
    }

    public static bool LoadPropertyMarkDirty(IManageProperties target, IPropertyInfo propertyInfo, object? newValue)
    {
      if (propertyInfo is ITypedPropertyLoader loader && loader.TryLoadPropertyMarkDirty(target, newValue, out var valuesDiffer))
        return valuesDiffer;

      return (bool)InvokeGenericLoadMethod(target, "LoadPropertyMarkDirty", propertyInfo, newValue)!;
    }

    /// <summary>
    /// Fallback for IPropertyInfo implementations that don't derive from
    /// PropertyInfo{T}, or values that need the conversions reflection
    /// invocation applies (such as widening a boxed int to long).
    /// </summary>
    private static object? InvokeGenericLoadMethod(IManageProperties target, string methodName, IPropertyInfo propertyInfo, object? newValue)
    {
#if NET8_0_OR_GREATER
      if (RuntimeFeature.IsDynamicCodeSupported)
        return InvokeGenericLoadMethodByReflection(target, methodName, propertyInfo, newValue);

      throw new NotSupportedException(
        $"Loading property '{propertyInfo.Name}' with a value of type '{newValue?.GetType().FullName}' requires dynamic code, " +
        $"which is not available in this runtime. Use a PropertyInfo<{propertyInfo.Type.Name}> and a value of that type.");
#else
      return InvokeGenericLoadMethodByReflection(target, methodName, propertyInfo, newValue);
#endif
    }

    [RequiresDynamicCode("Creates a generic method instantiation for the property type at runtime.")]
    [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "The type argument is IPropertyInfo.Type, which PropertyInfo<T> preserves through DynamicallyAccessedMembers on T.")]
    private static object? InvokeGenericLoadMethodByReflection(IManageProperties target, string methodName, IPropertyInfo propertyInfo, object? newValue)
    {
      var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var method = target.GetType().GetMethods(flags).First(c => c.Name == methodName && c.IsGenericMethod);
      var gm = method.MakeGenericMethod(propertyInfo.Type);
      return gm.Invoke(target, [propertyInfo, newValue]);
    }
  }
}
