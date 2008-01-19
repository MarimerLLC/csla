using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Csla.Server
{
  internal class DataPortalMethodInfo
  {
    public bool RunLocal { get; private set; }
    public TransactionalTypes TransactionalType { get; private set; }

    public DataPortalMethodInfo(MethodInfo info)
    {
      this.RunLocal = IsRunLocal(info);
      this.TransactionalType = GetTransactionalType(info);
    }

    private static bool IsRunLocal(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(RunLocalAttribute), false);
    }

    private static bool IsTransactionalMethod(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(TransactionalAttribute));
    }

    private static TransactionalTypes GetTransactionalType(MethodInfo method)
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
