using System;
using System.Collections.Generic;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.Authorization
{
  [TestClass()]
  public class PerTypeAuthorizationTests
  {
    [TestMethod()]
    [ExpectedException(typeof(System.Security.SecurityException))]
    public void DenyWritePerType()
    {
      PerTypeAuthorization root = new PerTypeAuthorization();
      root.Test = "test";
    }

    [TestMethod()]
    [ExpectedException(typeof(System.Security.SecurityException))]
    public void DenyWritePerInstance()
    {
      PerTypeAuthorization root = new PerTypeAuthorization();
      root.Name = "test";
    }
  }

}
