//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Reflection;

namespace Csla.Server
{
  internal class DataPortalMethodInfo
  {
    public bool RunLocal { get; private set; }
    public TransactionalTypes TransactionalType { get; private set; }

    public DataPortalMethodInfo()
    {
      this.RunLocal = false;
      this.TransactionalType = TransactionalTypes.Manual;
    }

    public DataPortalMethodInfo(System.Reflection.MethodInfo info)
      : this()
    {
      if (info != null)
      {
        this.RunLocal = IsRunLocal(info);
        this.TransactionalType = GetTransactionalType(info);
      }
    }

    private static bool IsRunLocal(System.Reflection.MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(RunLocalAttribute), false);
    }

    private static bool IsTransactionalMethod(System.Reflection.MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(TransactionalAttribute));
    }

    private static TransactionalTypes GetTransactionalType(System.Reflection.MethodInfo method)
    {
      TransactionalTypes result;
      if (IsTransactionalMethod(method))
      {
        TransactionalAttribute attrib =
          (TransactionalAttribute)Attribute.GetCustomAttribute(
          method, typeof(TransactionalAttribute));
        result = attrib.TransactionType;
      }
      else
        result = TransactionalTypes.Manual;
      return result;
    }
  }
}