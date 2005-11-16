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

namespace Csla.Test.Auth
{
    [TestClass()]
    public class AuthTests
    {
        private DataPortal.DpRoot root = DataPortal.DpRoot.NewRoot();

        [TestMethod()]
        public void TestAuthCloneRules()
        {
            ApplicationContext.GlobalContext.Clear();

            Security.TestPrincipal.SimulateLogin();

            Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));

            #region "Pre Cloning Tests" 

            //Is it denying read properly?
            Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
                "Read should have been denied 1");

            //Is it denying write properly?
            root.DenyWriteOnProperty = "DenyWriteOnProperty"; 

            Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied 2");

            //Is it denying both read and write properly?
            Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
                "Read should have been denied 3");

            root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied 4");

            //Is it allowing both read and write properly?
            Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
                "Read should have been allowed 5");

            root.AllowReadWriteOnProperty = "No value";
            Assert.AreEqual("No value", root.Auth,
                "Write should have been allowed 6");

            #endregion 

            #region "After Cloning Tests"

            //Do they work under cloning as well?
            DataPortal.DpRoot NewRoot = root.Clone();

            ApplicationContext.GlobalContext.Clear();

            //Is it denying read properly?
            Assert.AreEqual("[DenyReadOnProperty] Can't read property", NewRoot.DenyReadOnProperty,
                "Read should have been denied 7");

            //Is it denying write properly?
            NewRoot.DenyWriteOnProperty = "DenyWriteOnProperty";

            Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", NewRoot.Auth,
                "Write should have been denied 8");

            //Is it denying both read and write properly?
            Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", NewRoot.DenyReadWriteOnProperty,
                "Read should have been denied 9");

            NewRoot.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", NewRoot.Auth,
                "Write should have been denied 10");

            //Is it allowing both read and write properly?
            Assert.AreEqual(NewRoot.AllowReadWriteOnProperty, NewRoot.Auth,
                "Read should have been allowed 11");

            NewRoot.AllowReadWriteOnProperty = "AllowReadWriteOnProperty";
            Assert.AreEqual("AllowReadWriteOnProperty", NewRoot.Auth,
                "Write should have been allowed 12");

            #endregion 

            Security.TestPrincipal.SimulateLogout();
        }

        [TestMethod()]
        public void TestAuthBeginEditRules()
        {
            ApplicationContext.GlobalContext.Clear();

            Security.TestPrincipal.SimulateLogin();

            Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));

            root.Data = "Something new";

            root.BeginEdit();

            #region "Pre-Testing"

            root.Data = "Something new 1";

            //Is it denying read properly?
            Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
                "Read should have been denied");

            //Is it denying write properly?
            root.DenyWriteOnProperty = "DenyWriteOnProperty";

            Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied");

            //Is it denying both read and write properly?
            Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
                "Read should have been denied");

            root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied");

            //Is it allowing both read and write properly?
            Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
                "Read should have been allowed");

            root.AllowReadWriteOnProperty = "No value";
            Assert.AreEqual("No value", root.Auth,
                "Write should have been allowed");

            #endregion 

            #region "Cancel Edit"

            //Cancel the edit and see if the authorization rules still work
            root.CancelEdit();

            //Is it denying read properly?
            Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
                "Read should have been denied");

            //Is it denying write properly?
            root.DenyWriteOnProperty = "DenyWriteOnProperty";

            Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied");

            //Is it denying both read and write properly?
            Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
                "Read should have been denied");

            root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied");

            //Is it allowing both read and write properly?
            Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
                "Read should have been allowed");

            root.AllowReadWriteOnProperty = "No value";
            Assert.AreEqual("No value", root.Auth,
                "Write should have been allowed");

            #endregion

            #region "Apply Edit"

            //Apply this edit and see if the authorization rules still work
            //Is it denying read properly?
            Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
                "Read should have been denied");

            //Is it denying write properly?
            root.DenyWriteOnProperty = "DenyWriteOnProperty";

            Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied");

            //Is it denying both read and write properly?
            Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
                "Read should have been denied");

            root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
                "Write should have been denied");

            //Is it allowing both read and write properly?
            Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
                "Read should have been allowed");

            root.AllowReadWriteOnProperty = "No value";
            Assert.AreEqual("No value", root.Auth,
                "Write should have been allowed");


            #endregion

            Security.TestPrincipal.SimulateLogout();
        }

        [TestMethod()]
        public void TestAuthorizationAfterEditCycle()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.Test.Security.PermissionsRoot pr = Csla.Test.Security.PermissionsRoot.NewPermissionsRoot();

            Csla.Test.Security.TestPrincipal.SimulateLogin();
            pr.FirstName = "something";

            pr.BeginEdit();
            pr.FirstName = "ba";
            pr.CancelEdit();
            Csla.Test.Security.TestPrincipal.SimulateLogout();

            Csla.Test.Security.PermissionsRoot prClone = pr.Clone();
            Csla.Test.Security.TestPrincipal.SimulateLogin();
            prClone.FirstName = "somethiansdfasdf";
            Csla.Test.Security.TestPrincipal.SimulateLogout();
        }

    }
}

