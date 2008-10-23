using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  public interface INotifyUnhandledAsyncException
  {
    event EventHandler<ErrorEventArgs> UnhandledAsyncException;
  }
}
