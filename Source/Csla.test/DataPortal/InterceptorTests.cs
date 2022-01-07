using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Configuration;
using Csla.TestHelpers;
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
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateContext(
        options => options
        .DataPortal()
        .AddServerSideDataPortal(config => 
        {
          config.AddInterceptorProvider<TestInterceptor>();
          config.RegisterActivator<TestActivator>();
        }
        ));
    }

    [TestMethod]
    public void CreateWithIntercept()
    {
      TestResults.Reinitialise();

      var obj = CreateInitializeRoot("abc");
      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InitializeRoot"), "Initialize should have run");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InitializeRoot"), "Complete should have run");
      Assert.AreEqual("CreateInstance", TestResults.GetResult("Activate1+InitializeRoot"), "CreateInstance should have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeRoot"), "InitializeInstance should have run");
      Assert.AreEqual("ResolveType", TestResults.GetResult("Activate4+InitializeRoot"), "ResolveType should have run");
    }

    [TestMethod]
    public void FetchWithIntercept()
    {
      TestResults.Reinitialise();

      var obj = GetInitializeRoot("abc");
      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InitializeRoot"), "Initialize should have run");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InitializeRoot"), "Complete should have run");
      Assert.AreEqual("CreateInstance", TestResults.GetResult("Activate1+InitializeRoot"), "CreateInstance should have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeRoot"), "InitializeInstance should have run");
    }

    [TestMethod]
    public void FetchListWithIntercept()
    {
      TestResults.Reinitialise();

      var obj = GetInitializeListRoot();
      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InitializeListRoot"), "Initialize should have run");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InitializeListRoot"), "Complete should have run");
      Assert.AreEqual("CreateInstance", TestResults.GetResult("Activate1+InitializeListRoot"), "CreateInstance (list) should have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeListRoot"), "InitializeInstance (list) should have run");

      Assert.AreEqual("CreateInstance", TestResults.GetResult("Activate1+InitializeRoot"), "CreateInstance should have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeRoot"), "InitializeInstance should have run");
    }

    [TestMethod]
    public void InterceptException()
    {
      TestResults.Reinitialise();

      try
      {
        var obj = GetInitializeRoot("boom");
      }
      catch
      { }
      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InitializeRoot"), "Initialize should have run");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InitializeRoot"), "Complete should have run");
      Assert.IsTrue(!string.IsNullOrEmpty(TestResults.GetResult("InterceptException+InitializeRoot")), "Complete should have exception");
    }

    [TestMethod]
    public void UpdateWithIntercept()
    {
      TestResults.Reinitialise();

      var obj = GetInitializeRoot("abc");
      TestResults.Reinitialise();

      obj.Name = "xyz";
      obj = obj.Save();

      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InitializeRoot"), "Initialize should have run");
      Assert.AreEqual("Update", TestResults.GetResult("InterceptOp1+InitializeRoot"), "Initialize op should be Update");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InitializeRoot"), "Complete should have run");
      Assert.AreEqual("Update", TestResults.GetResult("InterceptOp2+InitializeRoot"), "Complete op should be Update");
      Assert.IsFalse(TestResults.ContainsResult("Activate1+InitializeRoot"), "CreateInstance should not have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeRoot"), "InitializeInstance should have run");
    }

    [TestMethod]
    public void UpdateListWithIntercept()
    {
      TestResults.Reinitialise();

      var obj = GetInitializeListRoot();
      TestResults.Reinitialise();

      obj[0].Name = "xyz";
      obj = obj.Save();

      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InitializeListRoot"), "Initialize should have run");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InitializeListRoot"), "Complete should have run");
      Assert.IsFalse(TestResults.ContainsResult("Activate1+InitializeListRoot"), "CreateInstance (list) should not have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeListRoot"), "InitializeInstance (list) should have run");

      Assert.IsFalse(TestResults.ContainsResult("Activate1+InitializeRoot"), "CreateInstance should not have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InitializeRoot"), "InitializeInstance should have run");
    }

    [TestMethod]
    public void ExecuteCommandWithIntercept()
    {
      IDataPortal<InterceptorCommand> dataPortal = _testDIContext.CreateDataPortal<InterceptorCommand>();

      TestResults.Reinitialise();

      var obj = new InterceptorCommand();
      obj = dataPortal.Execute(obj);

      Assert.AreEqual("Execute", TestResults.GetResult("InterceptorCommand"), "Execute should have run");
      Assert.AreEqual("Initialize", TestResults.GetResult("Intercept1+InterceptorCommand"), "Initialize should have run");
      Assert.AreEqual("Execute", TestResults.GetResult("InterceptOp1+InterceptorCommand"), "Initialize op should be Execute");
      Assert.AreEqual("Complete", TestResults.GetResult("Intercept2+InterceptorCommand"), "Complete should have run");
      Assert.AreEqual("Execute", TestResults.GetResult("InterceptOp2+InterceptorCommand"), "Complete op should be Execute");
      Assert.IsFalse(TestResults.ContainsResult("Activate1+InterceptorCommand"), "CreateInstance should not have run");
      Assert.AreEqual("InitializeInstance", TestResults.GetResult("Activate2+InterceptorCommand"), "InitializeInstance should have run");
    }

    private InitializeRoot CreateInitializeRoot(string ident)
    {
      IDataPortal<InitializeRoot> dataPortal = _testDIContext.CreateDataPortal<InitializeRoot>();

      return dataPortal.Create(ident);
    }

    private InitializeRoot GetInitializeRoot(string ident)
    {
      IDataPortal<InitializeRoot> dataPortal = _testDIContext.CreateDataPortal<InitializeRoot>();

      return dataPortal.Fetch(ident);
    }

    private InitializeListRoot GetInitializeListRoot()
    {
      IDataPortal<InitializeListRoot> dataPortal = _testDIContext.CreateDataPortal<InitializeListRoot>();

      return dataPortal.Fetch();
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

    [Update]
		protected void DataPortal_Update()
    {
    }

    private void Child_Update()
    {
    }
  }

  [Serializable]
  public class InitializeListRoot : BusinessListBase<InitializeListRoot, InitializeRoot>
  {
    private void DataPortal_Fetch([Inject] IChildDataPortal<InitializeRoot> childDataPortal)
    {
      using (SuppressListChangedEvents)
      {
        Add(childDataPortal.FetchChild("abc"));
      }
    }

    [Update]
		protected void DataPortal_Update()
    {
      base.Child_Update();
    }
  }

  [Serializable]
  public class InterceptorCommand : CommandBase<InterceptorCommand>
  {
    [Execute]
		protected void DataPortal_Execute()
    {
      TestResults.Add("InterceptorCommand", "Execute");
    }
  }

  public class TestInterceptor : Csla.Server.IInterceptDataPortal
  {
    public void Initialize(Server.InterceptArgs e)
    {
      TestResults.Add("Intercept1+" + e.ObjectType.Name, "Initialize");
      TestResults.Add("InterceptOp1+" + e.ObjectType.Name, e.Operation.ToString());
    }

    public void Complete(Server.InterceptArgs e)
    {
      TestResults.Add("Intercept2+" + e.ObjectType.Name, "Complete");
      if (e.Exception != null)
        TestResults.Add("InterceptException+" + e.ObjectType.Name, e.Exception.Message);
      TestResults.Add("InterceptOp2+" + e.ObjectType.Name, e.Operation.ToString());
    }
  }

  public class TestActivator : Csla.Server.IDataPortalActivator
  {
    public object CreateInstance(Type requestedType)
    {
      TestResults.Add("Activate1+" + requestedType.Name, "CreateInstance");
      return Activator.CreateInstance(requestedType);
    }

    public void InitializeInstance(object obj)
    {
      TestResults.Add("Activate2+" + obj.GetType().Name, "InitializeInstance");
    }

    public void FinalizeInstance(object obj)
    {
      TestResults.Add("Activate3+" + obj.GetType().Name, "FinalizeInstance");
    }

    public Type ResolveType(Type requestedType)
    {
      TestResults.Add("Activate4+" + requestedType.Name, "ResolveType");
      return requestedType;
    }
  }
}
