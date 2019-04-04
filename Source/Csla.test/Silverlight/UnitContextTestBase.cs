//-----------------------------------------------------------------------
// <copyright file="UnitContextTestBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
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