using System.Diagnostics.CodeAnalysis;

#if !IOS
//-----------------------------------------------------------------------
// <copyright file="DynamicMethodHandle.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
namespace Csla.Reflection
{
  internal class DynamicMethodHandle
  {
    public string? MethodName { get; }
    public DynamicMethodDelegate? DynamicMethod { get; }
    public bool HasFinalArrayParam { get; }
    public int MethodParamsLength { get; }
    public Type? FinalArrayElementType { get; }
    public bool IsAsyncTask { get; }
    [MemberNotNullWhen(true, nameof(ConvertToTaskObjectMethod))]
    public bool IsAsyncTaskObject { get; }
    public System.Reflection.MethodInfo? ConvertToTaskObjectMethod { get; }

    public DynamicMethodHandle(System.Reflection.MethodInfo? info, params object?[]? parameters)
    {
      if (info == null)
      {
        DynamicMethod = null;
      }
      else
      {
        MethodName = info.Name;
        var infoParams = info.GetParameters();
        object?[]? inParams = null;
        if (parameters == null)
        {
          inParams = [null];
        }
        else
        {
          inParams = parameters;
        }
        var pCount = infoParams.Length;
        var isgeneric = info.ReturnType.IsGenericType;
        if (pCount > 0 &&
           ((pCount == 1 && infoParams[0].ParameterType.IsArray) ||
           (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)))
        {
          HasFinalArrayParam = true;
          MethodParamsLength = pCount;
          FinalArrayElementType = infoParams[pCount - 1].ParameterType;
        }
        IsAsyncTask = (info.ReturnType == typeof(Task));
        IsAsyncTaskObject = (isgeneric && (info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
        DynamicMethod = DynamicMethodHandlerFactory.CreateMethod(info);
        if (IsAsyncTaskObject)
        {
          ConvertToTaskObjectMethod = TaskConversionHelper.CreateTaskObjectConversionMethodInfo(info.ReturnType.GetGenericArguments()[0]);
        }
      }
    }
  }
}
#endif