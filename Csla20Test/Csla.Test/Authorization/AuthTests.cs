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

            Console.WriteLine("\n\n\n\n root = " + root.Data);
        }
    }
}

