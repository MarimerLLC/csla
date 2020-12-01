#if !IOS
//-----------------------------------------------------------------------
// <copyright file="DynamicMethodHandlerFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Delegate for a dynamic constructor method.</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Reflection;
#if !NETFX_CORE
using System.Reflection.Emit;
#endif
using Csla.Properties;
using System.Collections.Generic;

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
  public delegate object DynamicMethodDelegate(object target, object[] args);
  /// <summary>
  /// Delegate for getting a value.
  /// </summary>
  /// <param name="target">Target object.</param>
  /// <returns></returns>
  public delegate object DynamicMemberGetDelegate(object target);
  /// <summary>
  /// Delegate for setting a value.
  /// </summary>
  /// <param name="target">Target object.</param>
  /// <param name="arg">Argument value.</param>
  public delegate void DynamicMemberSetDelegate(object target, object arg);

  internal static class DynamicMethodHandlerFactory
  {
    public static DynamicCtorDelegate CreateConstructor(ConstructorInfo constructor)
    {
      if (constructor == null)
        throw new ArgumentNullException("constructor");
      if (constructor.GetParameters().Length > 0)
        throw new NotSupportedException(Resources.ConstructorsWithParametersNotSupported);

      Expression body = Expression.New(constructor);
#if NETFX_CORE
      if (constructor.DeclaringType.IsValueType())
#else
      if (constructor.DeclaringType.IsValueType)
#endif
      {
        body = Expression.Convert(body, typeof(object));
      }

      return Expression.Lambda<DynamicCtorDelegate>(body).Compile();
    }

    public static DynamicMethodDelegate CreateMethod(System.Reflection.MethodInfo method)
    {
      if (method == null)
        throw new ArgumentNullException("method");

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

      Expression instance = Expression.Convert(targetExpression, method.DeclaringType);
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
#if NETFX_CORE
      else if (method.ReturnType.IsValueType())
#else
      else if (method.ReturnType.IsValueType)
#endif
      {
        body = Expression.Convert(body, typeof(object));
      }

      var lambda = Expression.Lambda<DynamicMethodDelegate>(
        body,
        targetExpression,
        parametersExpression);

      return (DynamicMethodDelegate)lambda.Compile();
    }

    public static DynamicMemberGetDelegate CreatePropertyGetter(PropertyInfo property)
    {
      if (property == null)
        throw new ArgumentNullException("property");

      if (!property.CanRead) return null;

      var target = Expression.Parameter(typeof(object));
      Expression body = Expression.Property(
        Expression.Convert(target, property.DeclaringType),
        property);

#if NETFX_CORE
      if (property.PropertyType.IsValueType())
#else
      if (property.PropertyType.IsValueType)
#endif
      {
        body = Expression.Convert(body, typeof(object));
      }

      var lambda = Expression.Lambda<DynamicMemberGetDelegate>(
        body,
        target);

      return lambda.Compile();
    }

    public static DynamicMemberSetDelegate CreatePropertySetter(PropertyInfo property)
    {
      if (property == null)
        throw new ArgumentNullException("property");

      if (!property.CanWrite) return null;

      var target = Expression.Parameter(typeof(object));
      var val = Expression.Parameter(typeof(object));

      Expression body = Expression.Assign(
        Expression.Property(
          Expression.Convert(target, property.DeclaringType),
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
      if (field == null)
        throw new ArgumentNullException("field");

      var target = Expression.Parameter(typeof(object));
      Expression body = Expression.Field(
        Expression.Convert(target, field.DeclaringType),
        field);

#if NETFX_CORE
      if (field.FieldType.IsValueType())
#else
      if (field.FieldType.IsValueType)
#endif
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
      if (field == null)
        throw new ArgumentNullException("property");

      var target = Expression.Parameter(typeof(object));
      var val = Expression.Parameter(typeof(object));

      Expression body = Expression.Assign(
        Expression.Field(
          Expression.Convert(target, field.DeclaringType),
          field),
        Expression.Convert(val, field.FieldType));

      var lambda = Expression.Lambda<DynamicMemberSetDelegate>(
        body,
        target,
        val);

      return lambda.Compile();
    }

#if !NETFX_CORE && !IOS && !NETSTANDARD2_0 && !NET5_0
    private static void EmitCastToReference(ILGenerator il, Type type)
    {
      if (type.IsValueType)
        il.Emit(OpCodes.Unbox_Any, type);
      else
        il.Emit(OpCodes.Castclass, type);
    }
#endif
  }
}
#endif