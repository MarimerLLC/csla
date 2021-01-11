//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodCallerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Reflection;
using Microsoft.Extensions.DependencyInjection;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ServiceProviderInvocationTests
  {
    [TestInitialize]
    public void Initialize()
    {
      Csla.ApplicationContext.DefaultServiceProvider = null;
      Csla.ApplicationContext.CurrentServiceProvider = null;
    }

    [TestMethod]
    public async Task NoServiceProvider()
    {
      var obj = new TestMethods();
      var method = new ServiceProviderMethodInfo { MethodInfo = obj.GetType().GetMethod("Method1") };
      Assert.IsNotNull(method, "needed method");
      await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await ServiceProviderMethodCaller.CallMethodTryAsync(obj, method, new object[] { 123 }));
    }

    [TestMethod]
    public async Task WithServiceProvider()
    {
      IServiceCollection services = new ServiceCollection();
      services.AddSingleton<ISpeak, Dog>();
      Csla.ApplicationContext.DefaultServiceProvider = services.BuildServiceProvider();

      var obj = new TestMethods();
      var method = new ServiceProviderMethodInfo { MethodInfo = obj.GetType().GetMethod("GetSpeech") };
      Assert.IsNotNull(method, "needed method");
      var result = (string)await ServiceProviderMethodCaller.CallMethodTryAsync(obj, method, new object[] { 123 });
      Assert.AreEqual("Bark", result);
    }
  }

  public interface ISpeak
  {
    string Speak();
  }

  public class Dog : ISpeak
  {
    public string Speak()
    {
      return "Bark";
    }
  }

  public class TestMethods
  {
    public bool Method1(int id, [Inject] ISpeak speaker)
    {
      return speaker == null;
    }

    public string GetSpeech(int id, [Inject] ISpeak speaker)
    {
      return speaker.Speak();
    }
  }
}
