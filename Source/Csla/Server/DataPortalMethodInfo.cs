//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  internal class DataPortalMethodInfo
  {
    public bool RunLocal { get; }
#if !(ANDROID || IOS) 
    public TransactionalAttribute TransactionalAttribute { get; }
#else
    public TransactionalTypes TransactionalType { get; private set; }
#endif


    public DataPortalMethodInfo(System.Reflection.MethodInfo info)
    {
      RunLocal = IsRunLocal(info);
#if !(ANDROID || IOS) 
      TransactionalAttribute = GetTransactionalAttribute(info);
#else
      TransactionalType = TransactionalTypes.Manual;
#endif
    }

    private static bool IsRunLocal(System.Reflection.MethodInfo method)
      => Attribute.IsDefined(method, typeof(RunLocalAttribute), false);

#if !(ANDROID || IOS)
    private static bool IsTransactionalMethod(System.Reflection.MethodInfo method)
      => Attribute.IsDefined(method, typeof(TransactionalAttribute));

    private static TransactionalAttribute GetTransactionalAttribute(System.Reflection.MethodInfo method)
    {
      TransactionalAttribute result;
      if (IsTransactionalMethod(method))
      {
        result = (TransactionalAttribute)Attribute.GetCustomAttribute(method, typeof(TransactionalAttribute))!;
      }
      else
        result = new TransactionalAttribute(TransactionalTypes.Manual);
      return result;
    }
#endif
  }
}