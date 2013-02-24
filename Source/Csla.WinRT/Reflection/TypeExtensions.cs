﻿using System;
using System.Linq;
using System.Reflection;

namespace Csla.Reflection
{
  /// <summary>
  /// Binding flags.
  /// </summary>
  [Flags]
  public enum BindingFlags
  {
    /// <summary>
    /// Public members.
    /// </summary>
    Public = 1,
    /// <summary>
    /// Instance members.
    /// </summary>
    Instance = 2,
    /// <summary>
    /// Flatten inheritance hierarchy.
    /// </summary>
    FlattenHierarchy = 4,
    /// <summary>
    /// Non-Public members.
    /// </summary>
    NonPublic = 8,
    /// <summary>
    /// Static members.
    /// </summary>
    Static = 16,
    /// <summary>
    /// Declared members only.
    /// </summary>
    DeclaredOnly = 32
  }

  /// <summary>
  /// Extension and helper methods to help mitigate the
  /// changes between .NET Classic and .NET WinRT.
  /// </summary>
  public static class TypeExtensions
  {
    /// <summary>
    /// Gets the type's constructor.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static ConstructorInfo GetConstructor(this Type t, object a, object b, object c, object d)
    {
      var ti = t.GetTypeInfo();
      var m = ti.DeclaredConstructors.Where(r => r.GetParameters().Count() == 0);
      return m.FirstOrDefault();
    }

    /// <summary>
    /// Gets the type's generic arguments.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Type[] GetGenericArguments(this Type t)
    {
      return t.GetTypeInfo().GenericTypeArguments.ToArray();
    }

    /// <summary>
    /// Gets a method.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="methodName"></param>
    /// <param name="flags"></param>
    /// <param name="p1"></param>
    /// <param name="parameterTypes"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static System.Reflection.MethodInfo GetMethod(this Type t, string methodName, BindingFlags flags, object p1, Type[] parameterTypes, object p2)
    {
      var ti = t.GetTypeInfo();
      System.Reflection.MethodInfo result = null;
      while (ti != null)
      {
        result = ti.DeclaredMethods.
          Where(r => r.Name == methodName &&
                     r.GetParameters().Count() == parameterTypes.Count()).FirstOrDefault();
        if (result != null)
        {
          var resultParameters = result.GetParameters();
          for (int i = 0; i < resultParameters.Count() - 1; i++)
          {
            if (resultParameters[i].ParameterType != parameterTypes[i])
            {
              result = null;
              break;
            }
          }
        }
        if (result != null)
          break;
        if (ti.BaseType == null)
          break;
        ti = ti.BaseType.GetTypeInfo();
      }
      return result;
    }

    /// <summary>
    /// Gets a method.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="methodName"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static System.Reflection.MethodInfo GetMethod(this Type t, string methodName, BindingFlags flags)
    {
      return GetMethod(t, methodName);
    }

    /// <summary>
    /// Gets a method.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static System.Reflection.MethodInfo GetMethod(this Type t, string methodName)
    {
      var ti = t.GetTypeInfo();
      System.Reflection.MethodInfo result = null;
      while (ti != null)
      {
        result = ti.DeclaredMethods.Where(r => r.Name == methodName).FirstOrDefault();
        if (result != null)
          break;
        if (ti.BaseType == null)
          break;
        ti = ti.BaseType.GetTypeInfo();
      }
      return result;
    }

    /// <summary>
    /// Gets a field.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="fieldName"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static System.Reflection.FieldInfo GetField(this Type t, string fieldName, BindingFlags flags)
    {
      return t.GetTypeInfo().GetDeclaredField(fieldName);
    }

    /// <summary>
    /// Gets the declared fields.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static System.Reflection.FieldInfo[] GetFields(this Type t, BindingFlags flags)
    {
      if ((flags | BindingFlags.Static) > 0)
        return t.GetTypeInfo().DeclaredFields.Where(r => r.IsStatic).ToArray();
      else
        return t.GetTypeInfo().DeclaredFields.ToArray();
    }

    /// <summary>
    /// Gets the containing assembly.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Assembly Assembly(this Type t)
    {
      return t.GetTypeInfo().Assembly;
    }

    /// <summary>
    /// Gets a custom attribute.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    public static Attribute GetCustomAttribute(this Type t, Type attributeType)
    {
      var result = t.GetTypeInfo().GetCustomAttributes(attributeType, true)
        .Where(r => r.GetType().Equals(attributeType)).FirstOrDefault();
      return result;
    }

    /// <summary>
    /// Gets custom attributes.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="attributeType"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static Attribute[] GetCustomAttributes(this Type t, Type attributeType, bool inherit)
    {
      var result = t.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
      return result.ToArray();
    }

    /// <summary>
    /// Gets a value indicating whether the type is
    /// serializable.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsSerializable(this Type t)
    {
      var tinfo = t.GetTypeInfo();
      var result = tinfo.GetCustomAttributes(typeof(Csla.Serialization.SerializableAttribute), false);
      return (result != null && result.Count() > 0);
    }

    /// <summary>
    /// Gets a value indicating whether the type
    /// is assignable to the target type.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static bool IsAssignableFrom(this Type t, Type targetType)
    {
      var ti = t.GetTypeInfo();
      return ti.IsAssignableFrom(targetType.GetTypeInfo());
    }

    /// <summary>
    /// Gets a property.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static System.Reflection.PropertyInfo GetProperty(this Type t, string propertyName)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.Where(r => r.Name == propertyName).FirstOrDefault();
    }

    /// <summary>
    /// Gets a property.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="propertyName"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static System.Reflection.PropertyInfo GetProperty(this Type t, string propertyName, BindingFlags flags)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.Where(r => r.Name == propertyName).FirstOrDefault();
    }

    /// <summary>
    /// Gets declared properties.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static System.Reflection.PropertyInfo[] GetProperties(this Type t)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.ToArray();
    }

    /// <summary>
    /// Gets declared properties.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static System.Reflection.PropertyInfo[] GetProperties(this Type t, BindingFlags flags)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.ToArray();
    }

    /// <summary>
    /// Gets declared methods.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static System.Reflection.MethodInfo[] GetMethods(this Type t, BindingFlags flags)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredMethods.ToArray();
    }

    /// <summary>
    /// Gets the base type.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Type BaseType(this Type t)
    {
      return t.GetTypeInfo().BaseType;
    }

    /// <summary>
    /// Gets the type name.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string Name(this Type t)
    {
      return t.GetTypeInfo().Name;
    }

    /// <summary>
    /// Gets a value indicating whether this is a value type.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsValueType(this Type t)
    {
      return t.GetTypeInfo().IsValueType;
    }

    /// <summary>
    /// Gets a value indicating whether this is a primitive type.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsPrimitive(this Type t)
    {
      return t.GetTypeInfo().IsPrimitive;
    }

    /// <summary>
    /// Gets a value indicating whether this is a generic type.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsGenericType(this Type t)
    {
      return t.GetTypeInfo().IsGenericType;
    }

    /// <summary>
    /// Gets a value indicating whether this is an enum.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsEnum(this Type t)
    {
      return t.GetTypeInfo().IsEnum;
    }

    /// <summary>
    /// Gets the TypeCode value for a type.
    /// </summary>
    /// <param name="t">Type object.</param>
    /// <returns></returns>
    public static TypeCode GetTypeCode(Type t)
    {
      TypeCode result = TypeCode.Empty;
      if (t.Equals(typeof(bool)))
        result = TypeCode.Boolean;
      else if (t.Equals(typeof(string)))
        result = TypeCode.String;
      else if (t.Equals(typeof(byte)))
        result = TypeCode.Byte;
      else if (t.Equals(typeof(char)))
        result = TypeCode.Char;
      else if (t.Equals(typeof(DateTime)))
        result = TypeCode.DateTime;
      else if (t.Equals(typeof(decimal)))
        result = TypeCode.Decimal;
      else if (t.Equals(typeof(double)))
        result = TypeCode.Double;
      else if (t.Equals(typeof(Int16)))
        result = TypeCode.Int16;
      else if (t.Equals(typeof(Int32)))
        result = TypeCode.Int32;
      else if (t.Equals(typeof(Int64)))
        result = TypeCode.Int64;
      else if (t.Equals(typeof(UInt16)))
        result = TypeCode.UInt16;
      else if (t.Equals(typeof(UInt32)))
        result = TypeCode.UInt32;
      else if (t.Equals(typeof(UInt64)))
        result = TypeCode.UInt64;
      else if (t.Equals(typeof(sbyte)))
        result = TypeCode.SByte;
      else if (t.Equals(typeof(Single)))
        result = TypeCode.Single;
      else if (t.Equals(typeof(UInt64)))
        result = TypeCode.UInt64;
      else if (t.Equals(typeof(object)))
        result = TypeCode.Object;
      return result;
    }

    /// <summary>
    /// Gets custom attributes.
    /// </summary>
    /// <param name="pi"></param>
    /// <param name="attributeType"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static Attribute[] GetCustomAttributes(this System.Reflection.PropertyInfo pi, Type attributeType, bool inherit)
    {
      //attributeType.IsAssignableFrom(pi.CustomAttributes.First().AttributeType)
      var result = pi.GetCustomAttributes().Where(r => attributeType.IsAssignableFrom(r.GetType())).ToArray();
      return result;
    }
  }

  /// <summary>
  /// Type codes
  /// </summary>
  public enum TypeCode
  {
    /// <summary>
    /// Empty
    /// </summary>
    Empty,
    /// <summary>
    /// Object
    /// </summary>
    Object,
    /// <summary>
    /// DBNull
    /// </summary>
    DBNull,
    /// <summary>
    /// Boolean
    /// </summary>
    Boolean,
    /// <summary>
    /// Char
    /// </summary>
    Char,
    /// <summary>
    /// SByte
    /// </summary>
    SByte,
    /// <summary>
    /// Byte
    /// </summary>
    Byte,
    /// <summary>
    /// Int16
    /// </summary>
    Int16,
    /// <summary>
    /// UInt16
    /// </summary>
    UInt16,
    /// <summary>
    /// Int32
    /// </summary>
    Int32,
    /// <summary>
    /// UInt32
    /// </summary>
    UInt32,
    /// <summary>
    /// Int64
    /// </summary>
    Int64,
    /// <summary>
    /// UInt64
    /// </summary>
    UInt64,
    /// <summary>
    /// Single
    /// </summary>
    Single,
    /// <summary>
    /// Double
    /// </summary>
    Double,
    /// <summary>
    /// Decimal
    /// </summary>
    Decimal,
    /// <summary>
    /// DateTime
    /// </summary>
    DateTime,
    /// <summary>
    /// String
    /// </summary>
    String
  }
}
