using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerDemo.Security
{
  [Serializable]
  public class MyIdentity : Csla.Security.CslaIdentity
  {
    public MyIdentity()
    {
      
    }

    public MyIdentity(string name)
    {
      this.Name = name;
    }
  }
}
