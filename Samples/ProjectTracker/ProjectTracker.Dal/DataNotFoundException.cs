using System;

namespace ProjectTracker.Dal
{
  [Serializable]
  public class DataNotFoundException : Exception
  {
    public DataNotFoundException(string message)
      : base(message)
    { }

    public DataNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    { }
  }
}
