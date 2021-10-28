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
using Csla.TestHelpers;
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
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.CreateChildAsync();
      Assert.AreEqual("none", child.Name);
    }

    [TestMethod]
    public async Task CreateChildInt32Criteria()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.CreateChildAsync(123);
      Assert.AreEqual("Int32", child.Name);
    }

    [TestMethod]
    public async Task CreateChildMultipleCriteria()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.CreateChildAsync("abc", 123);
      Assert.AreEqual("2", child.Name);
    }

    [TestMethod]
    public async Task FetchChildNoCriteria()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.FetchChildAsync();
      Assert.AreEqual("none", child.Name);
    }

    [TestMethod]
    public async Task FetchChildInt32Criteria()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.FetchChildAsync(123);
      Assert.AreEqual("Int32", child.Name);
    }

    [TestMethod]
    public async Task FetchChildMultipleCriteria()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.FetchChildAsync("abc", 123);
      Assert.AreEqual("2", child.Name);
    }

    [TestMethod]
    public async Task UpdateChild()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.FetchChildAsync();
      //await dp.UpdateChildAsync(child, "update", 123);
      Assert.AreEqual("update/123", child.Name);
    }

    [TestMethod]
    public async Task DeleteSelfChild()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChild>();
      var child = await dp.FetchChildAsync();
      child.MarkForDeletion();
      //await dp.UpdateChildAsync(child, "deleteme", 123);
      Assert.AreEqual("deleteme", child.Name);
    }

    [TestMethod]
    public async Task DeleteSelfChildNoParamFallback()
    {
      var dp = DataPortalFactory.CreateChildDataPortal<TestChildDeleteFallback>();
      var child = await dp.FetchChildAsync();
      child.MarkForDeletion();
      //await dp.UpdateChildAsync(child, "update", 123);
      Assert.AreEqual("deleted", child.Name);
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

    public void MarkForDeletion()
    {
      MarkDeleted();
    }

    [CreateChild]
    private async Task CreateChild()
    {
      await Task.Delay(0);
      Name = "none";
      return;
    }

    [CreateChild]
    private async Task CreateChild(int i)
    {
      await Task.Delay(0);
      Name = "Int32";
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
    private async Task CreateChild(string s, int i)
    {
      await Task.Delay(0);
      Name = "2";
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
      else if (o is Int32)
        Name = "Int32";
      else
        Name = "bad";
      return;
    }

    [FetchChild]
    private async Task FetchChild(string s, int i)
    {
      await Task.Delay(0);
      Name = "2";
      return;
    }

    [UpdateChild]
    private async Task UpdateChild(string s, int i)
    {
      await Task.Delay(0);
      Name = $"{s}/{i}";
    }

    [InsertChild]
    private async Task InsertChild(params object[] parameters)
    {
      await Task.Delay(0);
      Name = parameters[0].ToString();
    }

    [DeleteSelfChild]
    private async Task DeleteChild(string s, int i)
    {
      await Task.Delay(0);
      Name = s;
    }
  }

  [Serializable]
  public class TestChildDeleteFallback : BusinessBase<TestChildDeleteFallback>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public void MarkForDeletion()
    {
      MarkDeleted();
    }

    [FetchChild]
    private async Task FetchChild()
    {
      await Task.Delay(0);
      Name = "none";
      return;
    }

    [DeleteSelfChild]
    private void DeleteSelf()
    {
      Name = "deleted";
    }
  }
}
