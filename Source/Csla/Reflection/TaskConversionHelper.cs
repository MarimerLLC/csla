namespace Csla.Reflection;

internal static class TaskConversionHelper
{
  /// <summary>
  /// Awaits a <see cref="Task{TResult}"/> whose result type is only known
  /// at runtime and returns its result as an object.
  /// </summary>
  /// <remarks>
  /// Task{T} is invariant, so it can't be cast to Task{object}. Reading
  /// the Result property avoids a runtime generic method instantiation,
  /// which isn't available under NativeAOT.
  /// </remarks>
  public static async Task<object?> ConvertTask(Task task)
  {
    await task;
    return MethodCaller.CallPropertyGetter(task, nameof(Task<object>.Result));
  }
}
