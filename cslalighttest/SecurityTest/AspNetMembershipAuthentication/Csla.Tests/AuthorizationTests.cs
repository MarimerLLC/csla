#if NUNIT
using System.Configuration.Provider;
using System.Reflection;
using System.Web.Security;
using NUnit.Framework;
using SilverlightClassLibrary;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using UnitDriven;

namespace Csla.Tests
{
  [TestClass]
  public class AuthorizationTests
  {
  }
}
