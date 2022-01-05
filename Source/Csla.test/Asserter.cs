//-----------------------------------------------------------------------
// <copyright file="UnitTestContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitDriven
{
  public class Asserter
  {
    public Asserter()
    {
    }

    public void IsNull<T>(T value)
    {
      Assert.IsNull(value);
    }

    public void IsNotNull<T>(T value)
    {
      Assert.IsNotNull(value);
    }

    public void IsTrue(bool condition)
    {
      Assert.IsTrue(condition);
    }

    public void IsTrue(bool condition, string message)
    {
      Assert.IsTrue(condition, message);
    }

    public void IsFalse(bool condition)
    {
      Assert.IsFalse(condition);
    }

    public void IsFalse(bool condition, string message)
    {
      Assert.IsFalse(condition, message);
    }

    public void AreEqual<T>(T expected, T actual)
    {
      Assert.AreEqual(expected, actual);
    }

    public void AreEqual<T>(T expected, T actual, string message)
    {
      Assert.AreEqual(expected, actual, message);
    }

    public void Try(Action p)
    {
      p.Invoke();
    }

    public async Task Try(Func<Task> p)
    {
      await p();
    }

    public void Success() 
    { 
    }

  }
}