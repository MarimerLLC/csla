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
        [TestMethod()]
        public void TestAuthRules()
        {
            ApplicationContext.GlobalContext.Clear();

            DataPortal.DpRoot root = DataPortal.DpRoot.NewRoot();

            root.AddAuthRules();

            //Is it denying read properly?
            string DeniedRead = root.DenyReadOnProperty;

            //Is it denying write properly?
            root.DenyWriteOnProperty = "Write has been allowed when it shouldn't";
            string DeniedWrite = root.Auth;

            //Is it denying both read and write properly?
            //We're checking this to see whether the methods are working properly
            //for a single method (i.e., is the property having both read and write 
            //denied, like it's supposed to).
            string DeniedReadWrite_Read = root.DenyReadWriteOnProperty;
            root.DenyReadWriteOnProperty = "Write has been allowed when it shouldn't";
            string DeniedReadWrite_Write = root.Auth;

            //Is it allowing read and write properly?
            string AllowReadWrite_Read = root.AllowReadWriteOnProperty;
            root.AllowReadWriteOnProperty = "Write was allowed";
            string AllowReadWrite_Write = root.Auth;

            ApplicationContext.GlobalContext.Clear();

            Assert.AreEqual("[DenyReadOnProperty] Can't read property", DeniedRead, 
                "Read should have been denied 1");
            
            Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", DeniedWrite,
                "Write should have been denied 2");

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", DeniedReadWrite_Read,
                "Read should have been denied 3");

            Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", DeniedReadWrite_Write,
                "Write should have been denied 4");

            Assert.AreEqual("AllowReadWriteOnProperty", AllowReadWrite_Read,
                "Read should have been allowed 5");

            Assert.AreEqual("Write was allowed", AllowReadWrite_Write,
                "Write shoudl have been allowed 6");
        }
    }
}

