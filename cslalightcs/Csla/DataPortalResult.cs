using System;

namespace Csla
{
  public interface IDataPortalResult 
  {
    object Object { get; }
    Exception Error { get; }
  }

  public class DataPortalResult<T> : EventArgs, IDataPortalResult
  {
    public T Object { get; private set; }
    public Exception Error { get; private set; }
    public object UserState { get; private set; }

    internal DataPortalResult(T obj, Exception ex, object userState)
    {
      this.Object = obj;
      this.Error = ex;
      this.UserState = userState;
    }

    #region IDataPortalResult Members

    object IDataPortalResult.Object
    {
      get { return this.Object; }
    }

    Exception IDataPortalResult.Error
    {
      get { return this.Error; }
    }

    #endregion
  }
}
