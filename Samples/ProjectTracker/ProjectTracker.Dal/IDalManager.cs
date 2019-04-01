using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public interface IDalManager : IDisposable
  {
    T GetProvider<T>() where T : class;
  }
}
