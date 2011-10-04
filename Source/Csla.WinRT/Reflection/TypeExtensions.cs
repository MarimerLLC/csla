using System;
using System.Linq;
using System.Reflection;

namespace Csla.Reflection
{
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
  }
}
