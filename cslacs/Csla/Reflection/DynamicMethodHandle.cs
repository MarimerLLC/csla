using System;
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

    public DynamicMethodHandle(MethodInfo info, params object[] parameters)
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
        if (pCount > 0 &&
           ((pCount == 1 && infoParams[0].ParameterType.IsArray) ||
           (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)))
        {
          this.HasFinalArrayParam = true;
          this.MethodParamsLength = pCount;
          this.FinalArrayElementType = infoParams[pCount - 1].ParameterType;
        }
        this.DynamicMethod = DynamicMethodHandlerFactory.CreateMethod(info);
      }
    }
  }
}
