using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MethodCallercs
{

  public delegate object DynamicCtorDelegate();
  public delegate object DynamicMethodDelegate(object target, object[] args);

  internal static class DynamicMethodHandlerFactory
  {
    public static DynamicCtorDelegate CreateConstructor(ConstructorInfo constructor)
    {
      if (constructor == null)
        throw new ArgumentNullException("constructor");
      if (constructor.GetParameters().Length > 0)
        throw new NotSupportedException("Constructor with parameter(s) is not supported");

      DynamicMethod dm = new DynamicMethod(
          "ctor",
          constructor.DeclaringType,
          Type.EmptyTypes,
          true);

      ILGenerator il = dm.GetILGenerator();
      il.Emit(OpCodes.Nop);
      il.Emit(OpCodes.Newobj, constructor);
      il.Emit(OpCodes.Ret);

      return (DynamicCtorDelegate)dm.CreateDelegate(typeof(DynamicCtorDelegate));
    }

    public static DynamicMethodDelegate CreateMethod(MethodInfo method)
    {
      ParameterInfo[] pi = method.GetParameters();

      DynamicMethod dm = new DynamicMethod("dm", typeof(object),
          new Type[] { typeof(object), typeof(object[]) },
          typeof(DynamicMethodHandlerFactory), true);

      ILGenerator il = dm.GetILGenerator();

      il.Emit(OpCodes.Ldarg_0);

      for (int index = 0; index < pi.Length; index++)
      {
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldc_I4, index);

        Type parameterType = pi[index].ParameterType;
        if (parameterType.IsByRef)
        {
          parameterType = parameterType.GetElementType();
          if (parameterType.IsValueType)
          {
            il.Emit(OpCodes.Ldelem_Ref);
            il.Emit(OpCodes.Unbox, parameterType);
          }
          else
          {
            il.Emit(OpCodes.Ldelema, parameterType);
          }
        }
        else
        {
          il.Emit(OpCodes.Ldelem_Ref);

          if (parameterType.IsValueType)
          {
            il.Emit(OpCodes.Unbox, parameterType);
            il.Emit(OpCodes.Ldobj, parameterType);
          }
        }
      }

      if ((method.IsAbstract || method.IsVirtual)
          && !method.IsFinal && !method.DeclaringType.IsSealed)
      {
        il.Emit(OpCodes.Callvirt, method);
      }
      else
      {
        il.Emit(OpCodes.Call, method);
      }

      if (method.ReturnType == typeof(void))
      {
        il.Emit(OpCodes.Ldnull);
      }
      else if (method.ReturnType.IsValueType)
      {
        il.Emit(OpCodes.Box, method.ReturnType);
      }
      il.Emit(OpCodes.Ret);

      return (DynamicMethodDelegate)dm.CreateDelegate(typeof(DynamicMethodDelegate));
    }
  }
}
