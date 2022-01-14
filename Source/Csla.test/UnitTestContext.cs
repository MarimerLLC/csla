//-----------------------------------------------------------------------
// <copyright file="UnitTestContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitDriven
{
  public class UnitTestContext : IDisposable
  {

    public Asserter Assert { get; private set; } = new Asserter();

    public void Complete()
    { 
    }

    public void Dispose() { }

  }
}