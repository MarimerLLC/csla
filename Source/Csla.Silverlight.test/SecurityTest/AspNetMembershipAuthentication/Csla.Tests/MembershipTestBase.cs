#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using System.Configuration.Provider;
using System.Reflection;
using System.Web.Security;
using SilverlightClassLibrary;

using UnitDriven;

namespace Csla.Tests
{
  public abstract class MembershipTestBase : TestBase
  {
    #region Test Setup - Loading Membership Role Provider configuration

    //NUnit tests do not
    [SetUp]
    public void Setup()
    {
      InitMockMembershipProvider();
      InitMockRoleProvider();
    }

    private static void InitMockRoleProvider()
    {
      var role = new MockRoleProvider();
      role.Initialize("MockRoleProvider", null);
      typeof(ProviderCollection).GetField("_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Roles.Providers, false);
      Roles.Providers.Clear();
      Roles.Providers.Add(role);
    }

    private static void InitMockMembershipProvider()
    {
      //Little reflection to assure that the Membership.Providers can be modified
      var provider = new MockMembershipProvider();
      provider.Initialize("MockMembershipProvider", null);
      typeof(ProviderCollection).GetField("_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Membership.Providers, false);
      Membership.Providers.Clear();
      Membership.Providers.Add(provider);
    }

    #endregion
  }
}
