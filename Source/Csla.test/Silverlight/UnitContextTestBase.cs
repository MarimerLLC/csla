//-----------------------------------------------------------------------
// <copyright file="UnitContextTestBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitDriven;

namespace Csla.Test.Silverlight
{
  public class UnitContextTestBase : TestBase
  {
    protected UnitTestContext _context;

    [TestInitialize]
    public void SetUp()
    {
      _context = GetContext();
    }
    [TestCleanup]
    public void TearDown()
    {
      _context.Complete();  
    }
  }
}