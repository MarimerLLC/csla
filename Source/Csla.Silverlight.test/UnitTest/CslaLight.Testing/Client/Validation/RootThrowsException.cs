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

#if !SILVERLIGHT
    public static RootThrowsException NewRoot()
    {
      return Csla.DataPortal.Create<RootThrowsException>();
    }
#endif


    public static void NewRoot(EventHandler<DataPortalResult<RootThrowsException>> callback)
    {
      var portal = new DataPortal<RootThrowsException>();
      portal.CreateCompleted += callback;
      portal.BeginCreate();
    }

    protected override void AddBusinessRules()
    {
      System.Threading.Interlocked.Increment(ref _counter);
      throw new ArgumentException();
    }
  }
}
