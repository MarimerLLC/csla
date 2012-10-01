using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !NUNIT && !ANDROID
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif !ANDROID
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class InterceptorTests
  {
    [TestInitialize]
    public void Setup()
    {
      Csla.Server.DataPortal.InterceptorType = typeof(TestInterceptor);
    }

    [TestCleanup]
    public void Cleanup()
    {
      Csla.Server.DataPortal.InterceptorType = null;
    }

    [TestMethod]
    public void FetchWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = Csla.DataPortal.Fetch<InitializeRoot>("abc");
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2"].ToString(), "Complete should have run");
    }

    [TestMethod]
    public void InterceptException()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      try
      {
        var obj = Csla.DataPortal.Fetch<InitializeRoot>("boom");
      }
      catch
      { }
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2"].ToString(), "Complete should have run");
      Assert.IsTrue(!string.IsNullOrEmpty(Csla.ApplicationContext.GlobalContext["InterceptException"].ToString()), "Complete should have exception");
    }
  }

  public class InitializeRoot : BusinessBase<InitializeRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private void DataPortal_Fetch(string name)
    {
      if (name == "boom")
        throw new Exception("boom");

      using (BypassPropertyChecks)
      {
        Name = name;
      }
    }
  }

  public class TestInterceptor : Csla.Server.IInterceptDataPortal
  {
    public void Initialize(Server.InterceptArgs e)
    {
      Csla.ApplicationContext.GlobalContext["Intercept1"] = "Initialize";
    }

    public void Complete(Server.InterceptArgs e)
    {
      Csla.ApplicationContext.GlobalContext["Intercept2"] = "Complete";
      if (e.Exception != null)
        Csla.ApplicationContext.GlobalContext["InterceptException"] = e.Exception.Message;
    }
  }
}
