using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.TestHelpers
{
  public class TestDIContext
  {

    public TestDIContext(IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; private set; }

  }
}
