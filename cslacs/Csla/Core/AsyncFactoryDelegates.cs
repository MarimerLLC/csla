using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  // In case you need more flexibility than the standard delegates.
  public delegate void AsyncFactoryDelegate(Delegate completed, params object[] parameters);

  // Standard factory delegates
  public delegate void AsyncFactoryDelegate<R>(EventHandler<DataPortalResult<R>> completed);
  public delegate void AsyncFactoryDelegate<R, T>(EventHandler<DataPortalResult<R>> completed, T arg);
  public delegate void AsyncFactoryDelegate<R, T1, T2>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2);
  public delegate void AsyncFactoryDelegate<R, T1, T2, T3>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2, T3 arg3);
  public delegate void AsyncFactoryDelegate<R, T1, T2, T3, T4>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  public delegate void AsyncFactoryDelegate<R, T1, T2, T3, T4, T5>(EventHandler<DataPortalResult<R>> completed, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}
