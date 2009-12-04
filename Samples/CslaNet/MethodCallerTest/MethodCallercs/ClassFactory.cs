using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;

namespace MethodCallercs
{
  public class ClassFactory
  {
    bool eagerCaching = false;

    readonly Dictionary<Type, DynamicCtorDelegate> ctors = new Dictionary<Type, DynamicCtorDelegate>();
    public delegate object DynamicCtorDelegate();
    readonly Dictionary<CacheKey, DynamicMethodDelegate> mthods = new Dictionary<CacheKey, DynamicMethodDelegate>();
    delegate object DynamicMethodDelegate(object target, object[] args);

    private static BindingFlags ctorFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    private static BindingFlags mthodFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;

    public ClassFactory(bool eagerCaching)
    {
      this.eagerCaching = eagerCaching;
    }
    public ClassFactory() { }

    public object CreateInstance(Type type)
    {
      if (!ctors.ContainsKey(type))
      {
        ctors.Add(type, CreateDynamicConstructorHandler(type));
        if (eagerCaching)
          RegisterMethods(type);
      }
      return ctors[type]();
    }


    private DynamicCtorDelegate CreateDynamicConstructorHandler(Type type)
    {
      DynamicMethod dm = new DynamicMethod("Ctor", type, Type.EmptyTypes, typeof(ClassFactory).Module, true);
      ILGenerator ilgen = dm.GetILGenerator();
      ilgen.Emit(OpCodes.Nop);
      ilgen.Emit(OpCodes.Newobj, type.GetConstructor(ctorFlags, null, Type.EmptyTypes, null));
      ilgen.Emit(OpCodes.Ret);

      return (DynamicCtorDelegate)dm.CreateDelegate(typeof(DynamicCtorDelegate));
    }

    public object CallMethod(object obj, string methodName, params object[] parameters)
    {
      if (parameters == null)
        return DoCallMethod(obj, methodName, new object[] { });
      else
        return DoCallMethod(obj, methodName, parameters);
    }

    private object DoCallMethod(object obj, string methodName, object[] parameters)
    {
      Type type = obj.GetType();
      Type[] paramTypes = Array.ConvertAll<object, Type>(parameters, new Converter<object, Type>(delegate(object par)
      {
        if (par == null) return typeof(object);
        return par.GetType();
      }));

      DynamicMethodDelegate mthod;
      CacheKey key = new CacheKey(type, methodName, paramTypes);
      if (!mthods.TryGetValue(key, out mthod))
      {
        //mthod = CreateDynamicMethodHandler(type, methodName, paramTypes);
        mthod = CreateDynamicMethodHandler(type, methodName, parameters);
        mthods.Add(key, mthod);
      }

      return mthod(obj, parameters);
    }

    private void RegisterMethods(Type type)
    {
      Type currentType = type;
      do
      {
        foreach (MethodInfo info in currentType.GetMethods(mthodFlags))
        {
          mthods.Add(new CacheKey(info), CreateDynamicMethodHandler(info));
        }
        currentType = type.BaseType;
      }
      while (currentType != null && currentType != typeof(object));
    }

    private DynamicMethodDelegate CreateDynamicMethodHandler(Type type, string methodName, params object[] parameters) //, Type[] parameterTypes)
    {
      MethodInfo method = MethodCaller.GetMethod(type, methodName, parameters); // FindPrivateMethod(type, methodName, parameterTypes);
      return CreateDynamicMethodHandler(method);
    }

    private DynamicMethodDelegate CreateDynamicMethodHandler(MethodInfo method)
    {
      ParameterInfo[] pi = method.GetParameters();

      DynamicMethod dm = new DynamicMethod("", typeof(object),
          new Type[] { typeof(object), typeof(object[]) },
          typeof(ClassFactory), true);

      ILGenerator il = dm.GetILGenerator();

      il.Emit(OpCodes.Ldarg_0);

      for (int i = 0; i < pi.Length; i++)
      {
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldc_I4, i);

        Type parameterType = pi[i].ParameterType;
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

    public static MethodInfo FindPrivateMethod(Type type, string methodName, Type[] parameterTypes)
    {
      MethodInfo result = null;
      Type currentType = type;
      do
      {
        result = currentType.GetMethod(methodName, mthodFlags, null, parameterTypes, null);
        if (result != null)
          break;
        currentType = currentType.BaseType;
      }
      while (currentType != null && currentType != typeof(System.Object));
      return result;
    }
  }

  class CacheKey
  {
    private Type _classType;
    private string _methodName;
    private Type[] _typeParameters;
    public CacheKey(MethodInfo mi)
    {
      _classType = mi.DeclaringType;
      _methodName = mi.Name;
      _typeParameters = Array.ConvertAll<ParameterInfo, Type>(mi.GetParameters(), new Converter<ParameterInfo, Type>(delegate(ParameterInfo info)
      {
        return info.ParameterType;
      }));
    }
    public CacheKey(Type type, string methodName, Type[] typeParameters)
    {
      _classType = type;
      _methodName = methodName;
      _typeParameters = typeParameters;

    }
    static readonly int primeNumber = 37;
    public override int GetHashCode()
    {
      int result = primeNumber * _methodName.GetHashCode() + _classType.GetHashCode();

      for (int i = 0; i < _typeParameters.Length; i++)
      {
        result += primeNumber * _typeParameters[i].GetHashCode();
      }
      return result;
    }
    public override bool Equals(object obj)
    {
      if (this == obj) return true;
      if (obj == null) return false;
      CacheKey key1 = obj as CacheKey;
      if (key1 == null) return false;
      if (_classType != key1._classType || _methodName != key1._methodName) return false;

      if (_typeParameters.Length != key1._typeParameters.Length) return false;
      for (int i = 0; i < _typeParameters.Length; i++)
      {
        if (!Equals(_typeParameters[i], key1._typeParameters[i])) return false;
      }
      return true;
    }
  }
}
