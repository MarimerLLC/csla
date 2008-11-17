using System.Data;
using System.Data.SqlClient;
using Csla.Data;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using System.Configuration;
using System.Linq;
using Csla.Test.Basic;
#endif

namespace Csla.Test.CslaDataProvider
{
  [TestClass]
  public class CslaDataProviderTests
  {
    [TestMethod]
    public void TestAddNew()
    {
      
      Csla.Test.Basic.RootList list = new Csla.Test.Basic.RootList();
      Csla.Wpf.CslaDataProvider dp = new Csla.Wpf.CslaDataProvider();
      dp.ObjectInstance = list;
      RootListChild child = dp.AddNew() as RootListChild;
      Assert.IsNotNull(child);
    }

    [TestMethod]
    public void TestAddNewReturnsNull()
    {
      Csla.Test.Basic.Root item = Csla.Test.Basic.Root.NewRoot();
      Csla.Wpf.CslaDataProvider dp = new Csla.Wpf.CslaDataProvider();
      dp.ObjectInstance = item;
      object child = dp.AddNew();
      Assert.IsNull(child);
    }
  }
}
