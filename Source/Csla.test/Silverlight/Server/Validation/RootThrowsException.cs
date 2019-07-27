using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class RootThrowsException : BusinessBase<RootThrowsException>
  {
    private static int _counter = 0;
    public static int Counter
    {
      get { return _counter; }
      set { _counter = value; }
    }

    protected override void AddBusinessRules()
    {
      System.Threading.Interlocked.Increment(ref _counter);
      throw new ArgumentException();
    }
  }
}
