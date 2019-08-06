//-----------------------------------------------------------------------
// <copyright file="NullableTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.Nullable
{
    [TestClass()]
    public class NullableTests
    {
        [TestMethod()]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void TestNullableProperty()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            NullableObject nullRoot = NullableObject.NewNullableObject();
            nullRoot.NullableInteger = null;
            nullRoot.Name = null;
            Assert.AreEqual(null, nullRoot.Name);
            Assert.AreEqual(null, nullRoot.NullableInteger);
        }

        [TestMethod()]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void TestNullableField()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            NullableObject nullRoot = NullableObject.NewNullableObject();
            nullRoot._nullableIntMember = null;
            Assert.AreEqual(null, nullRoot._nullableIntMember);
        }

        [TestMethod()]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public void TestNullableAfterClone()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            NullableObject nullRoot = NullableObject.NewNullableObject();
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
            Csla.ApplicationContext.GlobalContext.Clear();
            NullableObject nullRoot = NullableObject.NewNullableObject();
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