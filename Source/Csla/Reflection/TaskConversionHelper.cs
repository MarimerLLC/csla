using System.Diagnostics.CodeAnalysis;

namespace Csla.Reflection;

internal static class TaskConversionHelper
{
  /// <summary>
  /// Awaits a <see cref="Task{TResult}"/> whose result type is only known
  /// at runtime and returns its result as an object.
  /// </summary>
  /// <remarks>
  /// Task{T} is invariant, so it can't be cast to Task{object}. Reading
  /// the Result property of the Task{T} base type avoids a runtime generic
  /// method instantiation, which isn't available under NativeAOT.
  /// </remarks>
  [DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(Task<>))]
  public static async Task<object?> ConvertTask(Task task)
  {
    await task;
    return GetResult(task);
  }

  [UnconditionalSuppressMessage("Trimming", "IL2075",
    Justification = "Result is a public property of Task<T>, preserved by the DynamicDependency on ConvertTask.")]
  private static object? GetResult(Task task)
  {
    // the runtime type is often a derived async state machine box; find the Task<T> base
    var type = task.GetType();
    while (!(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)))
      type = type.BaseType ?? throw new NotSupportedException($"{task.GetType()} is not a Task<T>.");

    var result = type.GetProperty(nameof(Task<object>.Result))
      ?? throw new NotSupportedException($"{type}.Result is not available; it may have been removed by trimming.");
    return result.GetValue(task);
  }
}
