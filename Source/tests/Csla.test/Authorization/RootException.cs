﻿namespace Csla.Test.Authorization
{
  public class RootException : BusinessBase<RootException>
  {
    private static int _counter = 0;
    public static int Counter
    {
      get { return _counter; }
      set { _counter = value; }
    }

    public static void AddObjectAuthorizationRules()
    {
      Interlocked.Increment(ref _counter);
      throw new ArgumentException();
    }
  }
}
