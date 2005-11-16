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

namespace Csla.Test.ValidationRules
{
    [TestClass()]
    public class ValidationTests
    {
        [TestMethod()]
        public void TestValidationRulesWithPrivateMember()
        {
            //this uses the first HasRulesManager which is the same as
            //HasRulesManager2 EXCEPT that it assigns the criteria data 
            //to the private variable that the Name property references
            //inside DataPortal_Create
            Csla.ApplicationContext.GlobalContext.Clear();
            HasRulesManager root = HasRulesManager.NewHasRulesManager();
            Assert.AreEqual("<new>", root.Name);
            Assert.AreEqual(false, root.IsValid, "should not be valid");
            Assert.AreEqual(1, root.BrokenRulesCollection.Count);
            Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        }

        [TestMethod()]
        public void TestValidationRulesWithPublicProperty()
        {
            //this is the same test as above except it uses HasRulesManager2,
            //which assigns the value from the criteria object to the Public 
            //Name property inside DataPortal_Create
            Csla.ApplicationContext.GlobalContext.Clear();
            HasRulesManager2 root = HasRulesManager2.NewHasRulesManager2();
            Assert.AreEqual("<new>", root.Name);
            Assert.AreEqual(false, root.IsValid, "should not be valid");
            Assert.AreEqual(1, root.BrokenRulesCollection.Count);
            Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        }

        [TestMethod()]
        public void BreakRequiredRule()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            HasRulesManager root = HasRulesManager.NewHasRulesManager();
            Assert.AreEqual(false, root.IsValid, "should not be valid");
            Assert.AreEqual(1, root.BrokenRulesCollection.Count);
            Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        }

        [TestMethod()]
        public void BreakLengthRule()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            HasRulesManager root = HasRulesManager.NewHasRulesManager();
            root.Name = "12345678901";
            Assert.AreEqual(false, root.IsValid, "should not be valid");
            Assert.AreEqual(1, root.BrokenRulesCollection.Count);
            //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description);
            Assert.AreEqual("The value for Name is too long", root.BrokenRulesCollection[0].Description);

            root.Name = "1234567890";
            Assert.AreEqual(true, root.IsValid, "should be valid");
            Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        }

        [TestMethod()]
        public void BreakLengthRuleAndClone()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            HasRulesManager root = HasRulesManager.NewHasRulesManager();
            root.Name = "12345678901";
            Assert.AreEqual(false, root.IsValid, "should not be valid");
            Assert.AreEqual(1, root.BrokenRulesCollection.Count);
            //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description;
            Assert.AreEqual("The value for Name is too long", root.BrokenRulesCollection[0].Description);

            root = (HasRulesManager)(root.Clone());
            Assert.AreEqual(false, root.IsValid, "should not be valid");
            Assert.AreEqual(1, root.BrokenRulesCollection.Count);
            //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description;
            Assert.AreEqual("The value for Name is too long", root.BrokenRulesCollection[0].Description);

            root.Name = "1234567890";
            Assert.AreEqual(true, root.IsValid, "Should be valid");
            Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        }
    }
}
