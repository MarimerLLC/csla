//-----------------------------------------------------------------------
// <copyright file="FindMethodHierarchyTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests MethodCaller.FindMethod walking the inheritance hierarchy</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.MethodCaller
{
  [TestClass]
  public class FindMethodHierarchyTests
  {
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    [TestMethod]
    [Timeout(10000)]
    public void FindMethod_FindsPrivateMethodTwoLevelsUp()
    {
      var method = Csla.Reflection.MethodCaller.FindMethod(typeof(FindLeaf), "Hidden", PrivateInstance);

      Assert.IsNotNull(method);
      Assert.AreEqual(typeof(FindRoot), method.DeclaringType);
    }

    [TestMethod]
    [Timeout(10000)]
    public void FindMethod_ReturnsNullWhenMethodDoesNotExist()
    {
      var method = Csla.Reflection.MethodCaller.FindMethod(typeof(FindLeaf), "DoesNotExist", PrivateInstance);

      Assert.IsNull(method);
    }
  }

  public class FindRoot
  {
    private void Hidden()
    { }
  }

  public class FindMiddle : FindRoot;

  public class FindLeaf : FindMiddle;
}
