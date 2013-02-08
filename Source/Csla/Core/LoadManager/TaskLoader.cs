using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Core.LoadManager
{

  internal class TaskDataPortalResult : IDataPortalResult
  {
    public object Object { get; internal set; }
    public Exception Error { get; internal set; }
    public object UserState { get; internal set; }
  }

  /// <summary>
  /// Wraps async loading by Task&gt;T&lt; from DataPortal.XYZAsync methods
  /// </summary>
  internal class TaskLoader<T> : IAsyncLoader
  {
    private readonly Task<T> _loader;
    public IPropertyInfo Property { get; private set; }

    public TaskLoader(IPropertyInfo property, Task<T> loader)
    {
      Property = property;
      _loader = loader;
    }

    public async void Load(Action<IAsyncLoader, IDataPortalResult> callback)
    {
      TaskDataPortalResult result;
      try
      {
        var o = await _loader;
        result = new TaskDataPortalResult() { Error = null, Object = o };
      }
      catch (Exception ex)
      {
        result = new TaskDataPortalResult() { Error = ex, Object = null };
      }

      callback(this, result);
    }
  }
}
