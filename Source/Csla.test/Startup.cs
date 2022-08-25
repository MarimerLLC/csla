using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test
{
  [TestClass]
  public class Startup
  {

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory;
      if (path.EndsWith(@"\"))
      {
        path = path.Substring(0, path.Length - 1);
      }
      AppDomain.CurrentDomain.SetData("DataDirectory", path);
    }
  }
}
