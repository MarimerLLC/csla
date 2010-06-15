//-----------------------------------------------------------------------
// <copyright file="RollbackTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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

namespace Csla.Test.Silverlight.Rollback
{
#if SILVERLIGHT
  [TestClass]
#endif
  public class RollbackTests : TestBase
  {
#if SILVERLIGHT
    [TestMethod]
    public void UnintializedProperty_RollsBack_AfterCancelEdit()
    {
      var context = GetContext();

      RollbackRoot.BeginCreateLocal((o, e) =>
      {
        context.Assert.Try(() =>
          {
            var item = e.Object;
            var initialValue = 0;// item.UnInitedP;

            item.BeginEdit();
            item.UnInitedP = 1000;
            item.Another = "test";
            item.CancelEdit();

            context.Assert.AreEqual(initialValue, item.UnInitedP);
            context.Assert.Success();
          });
      });

      context.Complete();
    }
#endif

  }
}