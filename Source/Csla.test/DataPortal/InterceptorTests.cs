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
      Csla.ApplicationContext.DataPortalActivator = new TestActivator();
    }

    [TestCleanup]
    public void Cleanup()
    {
      Csla.Server.DataPortal.InterceptorType = null;
      Csla.ApplicationContext.DataPortalActivator = null;
    }

    [TestMethod]
    public void CreateWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = Csla.DataPortal.Create<InitializeRoot>("abc");
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InitializeRoot"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InitializeRoot"].ToString(), "Complete should have run");
      Assert.AreEqual("CreateInstance", Csla.ApplicationContext.GlobalContext["Activate1+InitializeRoot"].ToString(), "CreateInstance should have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeRoot"].ToString(), "InitializeInstance should have run");
      Assert.AreEqual("ResolveType", Csla.ApplicationContext.GlobalContext["Activate4+InitializeRoot"].ToString(), "ResolveType should have run");
    }

    [TestMethod]
    public void FetchWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = Csla.DataPortal.Fetch<InitializeRoot>("abc");
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InitializeRoot"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InitializeRoot"].ToString(), "Complete should have run");
      Assert.AreEqual("CreateInstance", Csla.ApplicationContext.GlobalContext["Activate1+InitializeRoot"].ToString(), "CreateInstance should have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeRoot"].ToString(), "InitializeInstance should have run");
    }

    [TestMethod]
    public void FetchListWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = Csla.DataPortal.Fetch<InitializeListRoot>();
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InitializeListRoot"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InitializeListRoot"].ToString(), "Complete should have run");
      Assert.AreEqual("CreateInstance", Csla.ApplicationContext.GlobalContext["Activate1+InitializeListRoot"].ToString(), "CreateInstance (list) should have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeListRoot"].ToString(), "InitializeInstance (list) should have run");

      Assert.AreEqual("CreateInstance", Csla.ApplicationContext.GlobalContext["Activate1+InitializeRoot"].ToString(), "CreateInstance should have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeRoot"].ToString(), "InitializeInstance should have run");
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
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InitializeRoot"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InitializeRoot"].ToString(), "Complete should have run");
      Assert.IsTrue(!string.IsNullOrEmpty(Csla.ApplicationContext.GlobalContext["InterceptException+InitializeRoot"].ToString()), "Complete should have exception");
    }

    [TestMethod]
    public void UpdateWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = Csla.DataPortal.Fetch<InitializeRoot>("abc");
      Csla.ApplicationContext.GlobalContext.Clear();

      obj.Name = "xyz";
      obj = obj.Save();

      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InitializeRoot"].ToString(), "Initialize should have run");
      Assert.AreEqual("Update", Csla.ApplicationContext.GlobalContext["InterceptOp1+InitializeRoot"].ToString(), "Initialize op should be Update");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InitializeRoot"].ToString(), "Complete should have run");
      Assert.AreEqual("Update", Csla.ApplicationContext.GlobalContext["InterceptOp2+InitializeRoot"].ToString(), "Complete op should be Update");
      Assert.IsFalse(Csla.ApplicationContext.GlobalContext.Contains("Activate1+InitializeRoot"), "CreateInstance should not have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeRoot"].ToString(), "InitializeInstance should have run");
    }

    [TestMethod]
    public void UpdateListWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = Csla.DataPortal.Fetch<InitializeListRoot>();
      Csla.ApplicationContext.GlobalContext.Clear();

      obj[0].Name = "xyz";
      obj = obj.Save();

      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InitializeListRoot"].ToString(), "Initialize should have run");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InitializeListRoot"].ToString(), "Complete should have run");
      Assert.IsFalse(Csla.ApplicationContext.GlobalContext.Contains("Activate1+InitializeListRoot"), "CreateInstance (list) should not have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeListRoot"].ToString(), "InitializeInstance (list) should have run");

      Assert.IsFalse(Csla.ApplicationContext.GlobalContext.Contains("Activate1+InitializeRoot"), "CreateInstance should not have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InitializeRoot"].ToString(), "InitializeInstance should have run");
    }

    [TestMethod]
    public void ExecuteCommandWithIntercept()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      var obj = new InterceptorCommand();
      obj = Csla.DataPortal.Execute(obj);

      Assert.AreEqual("Execute", Csla.ApplicationContext.GlobalContext["InterceptorCommand"].ToString(), "Execute should have run");
      Assert.AreEqual("Initialize", Csla.ApplicationContext.GlobalContext["Intercept1+InterceptorCommand"].ToString(), "Initialize should have run");
      Assert.AreEqual("Execute", Csla.ApplicationContext.GlobalContext["InterceptOp1+InterceptorCommand"].ToString(), "Initialize op should be Execute");
      Assert.AreEqual("Complete", Csla.ApplicationContext.GlobalContext["Intercept2+InterceptorCommand"].ToString(), "Complete should have run");
      Assert.AreEqual("Execute", Csla.ApplicationContext.GlobalContext["InterceptOp2+InterceptorCommand"].ToString(), "Complete op should be Execute");
      Assert.IsFalse(Csla.ApplicationContext.GlobalContext.Contains("Activate1+InterceptorCommand"), "CreateInstance should not have run");
      Assert.AreEqual("InitializeInstance", Csla.ApplicationContext.GlobalContext["Activate2+InterceptorCommand"].ToString(), "InitializeInstance should have run");
    }
  }

  [Serializable]
  public class InitializeRoot : BusinessBase<InitializeRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private void DataPortal_Create(string name)
    {
      Fetch(name);
    }

    private void DataPortal_Fetch(string name)
    {
      Fetch(name);
    }

    private void Child_Fetch(string name)
    {
      Fetch(name);
    }

    private void Fetch(string name)
    {
      if (name == "boom")
        throw new Exception("boom");

      using (BypassPropertyChecks)
      {
        Name = name;
      }
    }

    protected override void DataPortal_Update()
    {
    }

    private void Child_Update()
    {
    }
  }

  [Serializable]
  public class InitializeListRoot : BusinessListBase<InitializeListRoot, InitializeRoot>
  {
    private void DataPortal_Fetch()
    {
      using (SuppressListChangedEvents)
      {
        Add(Csla.DataPortal.FetchChild<InitializeRoot>("abc"));
      }
    }

    protected override void DataPortal_Update()
    {
      base.Child_Update();
    }
  }

  [Serializable]
  public class InterceptorCommand : CommandBase<InterceptorCommand>
  {
    protected override void DataPortal_Execute()
    {
      Csla.ApplicationContext.GlobalContext["InterceptorCommand"] = "Execute";
    }
  }

  public class TestInterceptor : Csla.Server.IInterceptDataPortal
  {
    public void Initialize(Server.InterceptArgs e)
    {
      Csla.ApplicationContext.GlobalContext["Intercept1+" + e.ObjectType.Name] = "Initialize";
      Csla.ApplicationContext.GlobalContext["InterceptOp1+" + e.ObjectType.Name] = e.Operation.ToString();
    }

    public void Complete(Server.InterceptArgs e)
    {
      Csla.ApplicationContext.GlobalContext["Intercept2+" + e.ObjectType.Name] = "Complete";
      if (e.Exception != null)
        Csla.ApplicationContext.GlobalContext["InterceptException+" + e.ObjectType.Name] = e.Exception.Message;
      Csla.ApplicationContext.GlobalContext["InterceptOp2+" + e.ObjectType.Name] = e.Operation.ToString();
    }
  }

  public class TestActivator : Csla.Server.IDataPortalActivator
  {
    public object CreateInstance(Type requestedType)
    {
      Csla.ApplicationContext.GlobalContext["Activate1+" + requestedType.Name] = "CreateInstance";
      return Activator.CreateInstance(requestedType);
    }

    public void InitializeInstance(object obj)
    {
      Csla.ApplicationContext.GlobalContext["Activate2+" + obj.GetType().Name] = "InitializeInstance";
    }

    public void FinalizeInstance(object obj)
    {
      Csla.ApplicationContext.GlobalContext["Activate3+" + obj.GetType().Name] = "FinalizeInstance";
    }

    public Type ResolveType(Type requestedType)
    {
      Csla.ApplicationContext.GlobalContext["Activate4+" + requestedType.Name] = "ResolveType";
      return requestedType;
    }
  }
}
