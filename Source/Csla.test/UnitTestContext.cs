//-----------------------------------------------------------------------
// <copyright file="UnitTestContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

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