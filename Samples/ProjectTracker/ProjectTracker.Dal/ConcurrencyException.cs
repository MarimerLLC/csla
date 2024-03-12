using System;

namespace ProjectTracker.Dal
{
  [Serializable]
  public class ConcurrencyException : Exception
  {
    public ConcurrencyException(string message)
      : base(message)
    { }

    public ConcurrencyException(string message, Exception innerException)
      : base(message, innerException)
    { }
  }
}
