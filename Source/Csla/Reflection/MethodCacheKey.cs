//-----------------------------------------------------------------------
// <copyright file="MethodCacheKey.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Reflection
{
  internal class MethodCacheKey
  {
    public string TypeName { get; }
    public string MethodName { get; }
    public Type[] ParamTypes { get; }
    private int _hashKey;

    public MethodCacheKey(string typeName, string methodName, Type[] paramTypes)
    {
      TypeName = typeName;
      MethodName = methodName;
      ParamTypes = paramTypes;

      _hashKey = typeName.GetHashCode();
      _hashKey ^= methodName.GetHashCode();
      foreach (Type item in paramTypes)
      {
        _hashKey ^= item.Name.GetHashCode();
      }
    }

    public override bool Equals(object? obj)
    {
      if (obj is MethodCacheKey key &&
          key.TypeName == TypeName &&
          key.MethodName == MethodName &&
          ArrayEquals(key.ParamTypes, ParamTypes))
        return true;

      return false;
    }

    private static bool ArrayEquals(Type[] a1, Type[] a2)
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