using System;
using System.Reflection;

namespace Csla
{
  internal static class Utilities
  {

    public static bool IsNumeric(object value)
    {
      double dbl;
      return double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any,
        System.Globalization.NumberFormatInfo.InvariantInfo, out dbl);
    }

    public static object CallByName(
      object target, string methodName, CallType callType, 
      params object[] args)
    {
      switch (callType)
      {
        case CallType.Get:
          {
            PropertyInfo p = target.GetType().GetProperty(methodName);
            return p.GetValue(target, args);
          }
        case CallType.Let:
        case CallType.Set:
          {
            PropertyInfo p = target.GetType().GetProperty(methodName);
            object[] index = null;
            args.CopyTo(index, 1);
            p.SetValue(target, args[0], index);
            return null;
          }
        case CallType.Method:
          {
            MethodInfo m = target.GetType().GetMethod(methodName);
            return m.Invoke(target, args);
          }
      }
      return null;
    }

    /// <summary>
    /// Returns a property's type, dealing with
    /// Nullable(Of T) if necessary.
    /// </summary>
    public static Type GetPropertyType(Type propertyType)
    {
      Type type = propertyType;
      if (type.IsGenericType &&
        (type.GetGenericTypeDefinition() == typeof(Nullable)))
        return type.GetGenericArguments()[0];
      return type;
    }

    /// <summary>
    /// Returns the type of child object
    /// contained in a collection or list.
    /// </summary>
    public static Type GetChildItemType(Type listType)
    {
      if (listType.IsArray)
        return listType.GetElementType();
      PropertyInfo[] props =
        listType.GetProperties(
          BindingFlags.FlattenHierarchy |
          BindingFlags.Public |
          BindingFlags.Instance);
      foreach (PropertyInfo item in props)
        if (Attribute.IsDefined(
          item, typeof(DefaultMemberAttribute)))
          return Utilities.GetPropertyType(
            item.PropertyType);
      return null;
    }
  }


  internal enum CallType
  {
    Get,
    Let,
    Method,
    Set
  }
}
