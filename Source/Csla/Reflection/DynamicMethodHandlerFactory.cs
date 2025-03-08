#if !IOS
//-----------------------------------------------------------------------
// <copyright file="DynamicMethodHandlerFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Delegate for a dynamic constructor method.</summary>
//-----------------------------------------------------------------------
using System.Linq.Expressions;
using System.Reflection;
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
using System.Reflection.Emit;
#endif
using Csla.Properties;

namespace Csla.Reflection
{
  /// <summary>
  /// Delegate for a dynamic constructor method.
  /// </summary>
  public delegate object DynamicCtorDelegate();
  /// <summary>
  /// Delegate for a dynamic method.
  /// </summary>
  /// <param name="target">
  /// Object containg method to invoke.
  /// </param>
  /// <param name="args">
  /// Parameters passed to method.
  /// </param>
  public delegate object DynamicMethodDelegate(object target, object?[] args);
  /// <summary>
  /// Delegate for getting a value.
  /// </summary>
  /// <param name="target">Target object.</param>
  /// <returns></returns>
  public delegate object? DynamicMemberGetDelegate(object target);
  /// <summary>
  /// Delegate for setting a value.
  /// </summary>
  /// <param name="target">Target object.</param>
  /// <param name="arg">Argument value.</param>
  public delegate void DynamicMemberSetDelegate(object target, object? arg);

  internal static class DynamicMethodHandlerFactory
  {
    public static DynamicCtorDelegate CreateConstructor(ConstructorInfo constructor)
    {
      Guard.NotNull(constructor);
      if (constructor.GetParameters().Length > 0)
        throw new NotSupportedException(Resources.ConstructorsWithParametersNotSupported);
      ThrowIfDeclaringTypeIsNull(constructor, nameof(constructor));

      Expression body = Expression.New(constructor);
      if (constructor.DeclaringType!.IsValueType)
      {
        body = Expression.Convert(body, typeof(object));
      }

      return Expression.Lambda<DynamicCtorDelegate>(body).Compile();
    }

    public static DynamicMethodDelegate CreateMethod(System.Reflection.MethodInfo method)
    {
      Guard.NotNull(method);
      ThrowIfDeclaringTypeIsNull(method, nameof(method));

      ParameterInfo[] pi = method.GetParameters();
      var targetExpression = Expression.Parameter(typeof(object));
      var parametersExpression = Expression.Parameter(typeof(object[]));

      Expression[] callParametrs = new Expression[pi.Length];
      for (int x = 0; x < pi.Length; x++)
      {
        callParametrs[x] =
          Expression.Convert(
            Expression.ArrayIndex(
              parametersExpression,
              Expression.Constant(x)),
            pi[x].ParameterType);
      }

      Expression instance = Expression.Convert(targetExpression, method.DeclaringType!);
      Expression body = pi.Length > 0
        ? Expression.Call(instance, method, callParametrs)
        : Expression.Call(instance, method);

      if (method.ReturnType == typeof(void))
      {
        var target = Expression.Label(typeof(object));
        var nullRef = Expression.Constant(null);
        body = Expression.Block(
          body,
           Expression.Return(target, nullRef),
          Expression.Label(target, nullRef));
      }
      else if (method.ReturnType.IsValueType)
      {
        body = Expression.Convert(body, typeof(object));
      }

      var lambda = Expression.Lambda<DynamicMethodDelegate>(
        body,
        targetExpression,
        parametersExpression);

      return lambda.Compile();
    }

    public static DynamicMemberGetDelegate? CreatePropertyGetter(PropertyInfo property)
    {
      Guard.NotNull(property);

      if (!property.CanRead) return null;

      ThrowIfDeclaringTypeIsNull(property, nameof(property));

      var target = Expression.Parameter(typeof(object));
      Expression body = Expression.Property(
        Expression.Convert(target, property.DeclaringType!),
        property);

      if (property.PropertyType.IsValueType)
      {
        body = Expression.Convert(body, typeof(object));
      }

      var lambda = Expression.Lambda<DynamicMemberGetDelegate>(
        body,
        target);

      return lambda.Compile();
    }

    public static DynamicMemberSetDelegate? CreatePropertySetter(PropertyInfo property)
    {
      Guard.NotNull(property);

      if (!property.CanWrite) return null;

      ThrowIfDeclaringTypeIsNull(property, nameof(property));

      var target = Expression.Parameter(typeof(object));
      var val = Expression.Parameter(typeof(object));

      Expression body = Expression.Assign(
        Expression.Property(
          Expression.Convert(target, property.DeclaringType!),
          property),
        Expression.Convert(val, property.PropertyType));

      var lambda = Expression.Lambda<DynamicMemberSetDelegate>(
        body,
        target,
        val);

      return lambda.Compile();
    }

    public static DynamicMemberGetDelegate CreateFieldGetter(FieldInfo field)
    {
      Guard.NotNull(field);
      ThrowIfDeclaringTypeIsNull(field, nameof(field));

      var target = Expression.Parameter(typeof(object));
      Expression body = Expression.Field(
        Expression.Convert(target, field.DeclaringType!),
        field);

      if (field.FieldType.IsValueType)
      {
        body = Expression.Convert(body, typeof(object));
      }

      var lambda = Expression.Lambda<DynamicMemberGetDelegate>(
        body,
        target);

      return lambda.Compile();
    }

    public static DynamicMemberSetDelegate CreateFieldSetter(FieldInfo field)
    {
      Guard.NotNull(field);
      ThrowIfDeclaringTypeIsNull(field, nameof(field));

      var target = Expression.Parameter(typeof(object));
      var val = Expression.Parameter(typeof(object));

      Expression body = Expression.Assign(
        Expression.Field(
          Expression.Convert(target, field.DeclaringType!),
          field),
        Expression.Convert(val, field.FieldType));

      var lambda = Expression.Lambda<DynamicMemberSetDelegate>(
        body,
        target,
        val);

      return lambda.Compile();
    }

#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
    private static void EmitCastToReference(ILGenerator il, Type type)
    {
      if (type.IsValueType)
        il.Emit(OpCodes.Unbox_Any, type);
      else
        il.Emit(OpCodes.Castclass, type);
    }
#endif

    private static void ThrowIfDeclaringTypeIsNull(MemberInfo info, string typeKind)
    {
      if (info.DeclaringType is null)
        throw new NotSupportedException(string.Format(Resources.MemberInfoDeclaringTypeMustBeNotNull, typeKind));
    }
  }
}
#endif