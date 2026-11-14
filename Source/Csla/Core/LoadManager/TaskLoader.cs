namespace Csla.Core.LoadManager
{

  internal class TaskDataPortalResult : IDataPortalResult
  {
    public object? Object { get; internal set; }
    public Exception? Error { get; internal set; }
    public object? UserState { get; internal set; }
  }

  /// <summary>
  /// Wraps async loading by Task&gt;T&lt; from DataPortal.XYZAsync methods
  /// </summary>
  internal class TaskLoader<T> : IAsyncLoader
  {
    private readonly Func<Task<T>> _loader;
    public IPropertyInfo Property { get; }

    public TaskLoader(IPropertyInfo property, Task<T> loader)
      : this(property, () => loader)
    { }

    /// <summary>
    /// Creates a loader that invokes <paramref name="loader"/> when
    /// <see cref="Load"/> runs, after the load manager has marked the
    /// property as loading.
    /// </summary>
    public TaskLoader(IPropertyInfo property, Func<Task<T>> loader)
    {
      Property = property;
      _loader = loader;
    }

    public async void Load(Action<IAsyncLoader, IDataPortalResult> callback)
    {
      TaskDataPortalResult result;
      try
      {
        // A factory that throws or returns null is reported like a failed load.
        var task = _loader() ?? throw new InvalidOperationException(
          $"The factory for lazy loaded property {Property.Name} returned null instead of a Task.");
        var o = await task;
        result = new TaskDataPortalResult { Error = null, Object = o };
      }
      catch (Exception ex)
      {
        result = new TaskDataPortalResult { Error = ex, Object = null };
      }

      callback(this, result);
    }
  }
}
