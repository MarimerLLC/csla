using System.Diagnostics.CodeAnalysis;
using Csla.Properties;

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

  [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = Core.TrimmingMessages.TaskResultJustification)]
  private static object? GetResult(Task task)
  {
    // the runtime type is often a derived async state machine box; find the Task<T> base
    var type = task.GetType();
    while (!(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)))
      type = type.BaseType ?? throw new NotSupportedException(string.Format(Resources.TaskOfTRequiredException, task.GetType()));

    var result = type.GetProperty(nameof(Task<object>.Result))
      ?? throw new NotSupportedException(string.Format(Resources.TaskResultNotAvailableException, type));
    return result.GetValue(task);
  }
}
