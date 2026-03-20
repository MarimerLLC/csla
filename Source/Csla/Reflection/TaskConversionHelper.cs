namespace Csla.Reflection;

internal static class TaskConversionHelper
{
  public static System.Reflection.MethodInfo CreateTaskObjectConversionMethodInfo(Type taskReturnType)
  {
    return typeof(TaskConversionHelper)
      .GetMethod(nameof(TaskConversionHelper.ConvertTask))!
      .MakeGenericMethod(taskReturnType);
  }
  
  public static async Task<object?> ConvertTask<T>(Task<T?> task)
  {
    return await task;
  }
}