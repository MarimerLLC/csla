using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitDriven
{
  public class TestBase
  {

    public UnitTestContext GetContext()
    {
      return new UnitTestContext();
    }

  }
}
