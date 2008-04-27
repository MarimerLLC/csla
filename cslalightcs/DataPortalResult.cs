using System;

namespace Csla
{
  public class DataPortalResult<T>
  {
    public T Object { get; private set; }
    public Exception Error { get; private set; }

    internal DataPortalResult(T obj, Exception ex)
    {
      this.Object = obj;
      this.Error = ex;
    }
  }
}
