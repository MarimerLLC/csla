#if !IOS
//-----------------------------------------------------------------------
// <copyright file="DynamicMethodHandle.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Reflection;

namespace Csla.Reflection
{
  internal class DynamicMethodHandle
  {
    public string MethodName { get; private set; }
    public DynamicMethodDelegate DynamicMethod { get; private set; }
    public bool HasFinalArrayParam { get; private set; }
    public int MethodParamsLength { get; private set; }
    public Type FinalArrayElementType { get; private set; }
    public bool IsAsyncTask { get; private set; }
    public bool IsAsyncTaskObject { get; private set; }

    public DynamicMethodHandle(System.Reflection.MethodInfo info, params object[] parameters)
    {
      if (info == null)
      {
        this.DynamicMethod = null;
      }
      else
      {
        this.MethodName = info.Name;
        var infoParams = info.GetParameters();
        object[] inParams = null;
        if (parameters == null)
        {
          inParams = new object[] { null };

        }
        else
        {
          inParams = parameters;
        }
        var pCount = infoParams.Length;
#if NETFX_CORE
        var isgeneric = info.ReturnType.IsGenericType();
        if (pCount > 0 &&
           ((pCount == 1 && infoParams[0].ParameterType.IsArray) ||
           (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Count() > 0)))
#else
        var isgeneric = info.ReturnType.IsGenericType;
        if (pCount > 0 &&
           ((pCount == 1 && infoParams[0].ParameterType.IsArray) ||
           (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)))
#endif
        {
          this.HasFinalArrayParam = true;
          this.MethodParamsLength = pCount;
          this.FinalArrayElementType = infoParams[pCount - 1].ParameterType;
        }
        IsAsyncTask = (info.ReturnType == typeof(System.Threading.Tasks.Task));
        IsAsyncTaskObject = (isgeneric && (info.ReturnType.GetGenericTypeDefinition() == typeof(System.Threading.Tasks.Task<>)));
        this.DynamicMethod = DynamicMethodHandlerFactory.CreateMethod(info);
      }
    }
  }
}
#endif