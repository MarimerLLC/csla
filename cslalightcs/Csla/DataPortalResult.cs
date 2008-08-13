using System;

namespace Csla
{
  public interface IDataPortalResult 
  {
    object Object { get; }
  }

  public class DataPortalResult<T> : EventArgs, IDataPortalResult
  {
    public T Object { get; private set; }
    public Exception Error { get; private set; }

    internal DataPortalResult(T obj, Exception ex)
    {
      this.Object = obj;
      this.Error = ex;
    }

    #region IDataPortalResult Members

    object IDataPortalResult.Object
    {
      get { return this.Object; }
    }

    #endregion
  }
}
