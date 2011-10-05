using System;
using System.Linq;
using System.Reflection;

namespace Csla.Reflection
{
  [Flags]
  public enum BindingFlags
  {
    Public = 1,
    Instance = 2,
    FlattenHierarchy = 4,
    NonPublic = 8,
    Static = 16,
    DeclaredOnly = 32
  }

  public static class TypeExtensions
  {
    public static ConstructorInfo GetConstructor(this Type t, object a, object b, object c, object d)
    {
      var ti = t.GetTypeInfo();
      var m = ti.DeclaredConstructors.Where(r => r.GetParameters().Count() == 0);
      return m.FirstOrDefault();
    }

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

    public static Attribute GetCustomAttribute(this Type t, Type attributeType)
    {
      var result = t.GetTypeInfo().GetCustomAttributes(attributeType, true)
        .Where(r => r.GetType().Equals(attributeType)).FirstOrDefault();
      return result;
    }

    public static Attribute[] GetCustomAttributes(this Type t, Type attributeType, bool inherit)
    {
      var result = t.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
      return result.ToArray();
    }

    public static bool IsSerializable(this Type t)
    {
      return t.GetTypeInfo().IsSerializable;
    }

    public static bool IsAssignableFrom(this Type t, Type targetType)
    {
      var ti = t.GetTypeInfo();
      return ti.IsAssignableFrom(targetType.GetTypeInfo());
    }

    public static System.Reflection.PropertyInfo GetProperty(this Type t, string propertyName)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.Where(r => r.Name == propertyName).FirstOrDefault();
    }

    public static System.Reflection.PropertyInfo GetProperty(this Type t, string propertyName, BindingFlags flags)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.Where(r => r.Name == propertyName).FirstOrDefault();
    }

    public static System.Reflection.PropertyInfo[] GetProperties(this Type t)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.ToArray();
    }

    public static System.Reflection.PropertyInfo[] GetProperties(this Type t, BindingFlags flags)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredProperties.ToArray();
    }

    public static System.Reflection.MethodInfo[] GetMethods(this Type t, BindingFlags flags)
    {
      var ti = t.GetTypeInfo();
      return ti.DeclaredMethods.ToArray();
    }

    public static Type BaseType(this Type t)
    {
      return t.GetTypeInfo().BaseType;
    }

    public static string Name(this Type t)
    {
      return t.GetTypeInfo().Name;
    }

    public static bool IsValueType(this Type t)
    {
      return t.GetTypeInfo().IsValueType;
    }

    public static bool IsPrimitive(this Type t)
    {
      return t.GetTypeInfo().IsPrimitive;
    }

    public static Attribute[] GetCustomAttributes(this System.Reflection.PropertyInfo pi, Type attributeType, bool inherit)
    {
      return pi.GetCustomAttributes().Where(r => r.GetType().Equals(attributeType)).ToArray();
    }
  }
}
