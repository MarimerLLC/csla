//-----------------------------------------------------------------------
// <copyright file="NullableTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.Nullable
{
  [TestClass()]
  public class NullableTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestNullableProperty()
    {
      IDataPortal<NullableObject> dataPortal = _testDIContext.CreateDataPortal<NullableObject>();

      NullableObject nullRoot = NullableObject.NewNullableObject(dataPortal);
      nullRoot.NullableInteger = null;
      nullRoot.Name = null;
      Assert.AreEqual(null, nullRoot.Name);
      Assert.AreEqual(null, nullRoot.NullableInteger);
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestNullableField()
    {
      IDataPortal<NullableObject> dataPortal = _testDIContext.CreateDataPortal<NullableObject>();

      NullableObject nullRoot = NullableObject.NewNullableObject(dataPortal);
      nullRoot._nullableIntMember = null;
      Assert.AreEqual(null, nullRoot._nullableIntMember);
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestNullableAfterClone()
    {
      IDataPortal<NullableObject> dataPortal = _testDIContext.CreateDataPortal<NullableObject>();

      NullableObject nullRoot = NullableObject.NewNullableObject(dataPortal);
      nullRoot._nullableIntMember = null;
      nullRoot.NullableInteger = null;
      NullableObject nullRoot2 = nullRoot.Clone();
      Assert.AreEqual(null, nullRoot2._nullableIntMember);
      Assert.AreEqual(null, nullRoot2.NullableInteger);
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestNullableAfterEditCycle()
    {
      IDataPortal<NullableObject> dataPortal = _testDIContext.CreateDataPortal<NullableObject>();

      NullableObject nullRoot = NullableObject.NewNullableObject(dataPortal);
      nullRoot.NullableInteger = null;
      nullRoot._nullableIntMember = null;

      nullRoot.BeginEdit();
      nullRoot.NullableInteger = 45;
      nullRoot._nullableIntMember = 32;
      nullRoot.ApplyEdit();

      Assert.AreEqual(45, nullRoot.NullableInteger);
      Assert.AreEqual(32, nullRoot._nullableIntMember);

      nullRoot.BeginEdit();
      nullRoot.NullableInteger = null;
      nullRoot._nullableIntMember = null;
      nullRoot.ApplyEdit();

      Assert.AreEqual(null, nullRoot.NullableInteger);
      Assert.AreEqual(null, nullRoot._nullableIntMember);

      nullRoot.BeginEdit();
      nullRoot.NullableInteger = 444;
      nullRoot._nullableIntMember = 222;
      nullRoot.CancelEdit();

      Assert.AreEqual(null, nullRoot.NullableInteger);
      Assert.AreEqual(null, nullRoot._nullableIntMember);
    }
  }
}