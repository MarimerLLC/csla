using System;
#if SILVERLIGHT
using Csla.Serialization;
#endif


namespace Csla.Test.Threading
{
  [Serializable]
  public class MyIdentity : Csla.Security.CslaIdentity
  {
    public MyIdentity(string name)
    {
      this.Name = name;
    }
  }
}
