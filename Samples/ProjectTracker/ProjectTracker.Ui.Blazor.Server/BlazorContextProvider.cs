using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProjectTracker.Ui.Blazor
{
  public class BlazorContextProvider : Csla.AspNetCore.ApplicationContextManager
  {
    public BlazorContextProvider(IServiceProvider provider)
      : base(provider)
    { }

  }
}
