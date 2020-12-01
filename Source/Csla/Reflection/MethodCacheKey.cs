//-----------------------------------------------------------------------
// <copyright file="MethodCacheKey.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Reflection
{
  internal class MethodCacheKey
  {
    public string TypeName { get; private set; }
    public string MethodName { get; private set; }
    public Type[] ParamTypes { get; private set; }
    private int _hashKey;

    public MethodCacheKey(string typeName, string methodName, Type[] paramTypes)
    {
      this.TypeName = typeName;
      this.MethodName = methodName;
      this.ParamTypes = paramTypes;

      _hashKey = typeName.GetHashCode();
      _hashKey = _hashKey ^ methodName.GetHashCode();
      foreach (Type item in paramTypes)
#if NETFX_CORE
        _hashKey = _hashKey ^ item.Name().GetHashCode();
#else
        _hashKey = _hashKey ^ item.Name.GetHashCode();
#endif
    }

    public override bool Equals(object obj)
    {
      MethodCacheKey key = obj as MethodCacheKey;
      if (key != null &&
          key.TypeName == this.TypeName &&
          key.MethodName == this.MethodName &&
          ArrayEquals(key.ParamTypes, this.ParamTypes))
        return true;

      return false;
    }

    private bool ArrayEquals(Type[] a1, Type[] a2)
    {
      if (a1.Length != a2.Length)
        return false;

      for (int pos = 0; pos < a1.Length; pos++)
        if (a1[pos] != a2[pos])
          return false;
      return true;
    }

    public override int GetHashCode()
    {
      return _hashKey;
    }
  }
}