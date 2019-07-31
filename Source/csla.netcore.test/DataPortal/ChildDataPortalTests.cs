//-----------------------------------------------------------------------
// <copyright file="ChildDataPortalTests.cs" company="Marimer LLC">
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
using Csla.Configuration;
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
  public class ChildDataPortalTests
  {
    [TestMethod]
    public async Task CreateChildNoCriteria()
    {
      var child = await Csla.DataPortal.CreateChildAsync<TestChild>();
      Assert.AreEqual("none", child.Name);
    }

    [TestMethod]
    public async Task CreateChildNullCriteria()
    {
      var dp = new Server.ChildDataPortal();
      var child = await dp.CreateAsync<TestChild>(null);
      Assert.AreEqual("null", child.Name);
    }

    [TestMethod]
    public async Task CreateChildInt32Criteria()
    {
      var dp = new Server.ChildDataPortal();
      var child = await dp.CreateAsync<TestChild>(123);
      Assert.AreEqual("Int32", child.Name);
    }

    [TestMethod]
    public async Task CreateChildMultipleCriteria()
    {
      var dp = new Server.ChildDataPortal();
      var child = await dp.CreateAsync<TestChild>(123, "abc");
      Assert.AreEqual("2", child.Name);
    }

    [TestMethod]
    public async Task FetchChildNoCriteria()
    {
      var child = await Csla.DataPortal.FetchChildAsync<TestChild>();
      Assert.AreEqual("none", child.Name);
    }

    [TestMethod]
    public async Task FetchChildNullCriteria()
    {
      var dp = new Server.ChildDataPortal();
      var child = await dp.FetchAsync<TestChild>(null);
      Assert.AreEqual("null", child.Name);
    }

    [TestMethod]
    public async Task FetchChildInt32Criteria()
    {
      var dp = new Server.ChildDataPortal();
      var child = await dp.FetchAsync<TestChild>(123);
      Assert.AreEqual("Int32", child.Name);
    }

    [TestMethod]
    public async Task FetchChildMultipleCriteria()
    {
      var dp = new Server.ChildDataPortal();
      var child = await dp.FetchAsync<TestChild>(123, "abc");
      Assert.AreEqual("2", child.Name);
    }
  }

  [Serializable]
  public class TestChild : BusinessBase<TestChild>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    [CreateChild]
    private async Task CreateChild()
    {
      await Task.Delay(0);
      Name = "none";
      return;
    }

    [CreateChild]
    private async Task CreateChild(object o)
    {
      await Task.Delay(0);
      if (o == null)
        Name = "null";
      else
        Name = "bad";
      return;
    }

    [CreateChild]
    private async Task CreateChild(params object[] parameters)
    {
      await Task.Delay(0);
      if (parameters == null)
        Name = "null array";
      else if (parameters.Length == 1)
        Name = parameters[0].GetType().Name;
      else
        Name = parameters.Length.ToString();
      return;
    }

    [FetchChild]
    private async Task FetchChild()
    {
      await Task.Delay(0);
      Name = "none";
      return;
    }

    [FetchChild]
    private async Task FetchChild(object o)
    {
      await Task.Delay(0);
      if (o == null)
        Name = "null";
      else
        Name = "bad";
      return;
    }

    [FetchChild]
    private async Task FetchChild(params object[] parameters)
    {
      await Task.Delay(0);
      if (parameters == null)
        Name = "null array";
      else if (parameters.Length == 1)
        Name = parameters[0].GetType().Name;
      else
        Name = parameters.Length.ToString();
      return;
    }
  }
}
