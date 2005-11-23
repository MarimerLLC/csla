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

namespace Csla.Test.SortedBindingList
{
    [TestClass()]
    public class SortedBindingListTests
    {
        [TestMethod]
        public void TestSortedBindingList()
        {
            string[] data = { "a", "c", "b", "z", "f" };
            SortedBindingList<string> list = new SortedBindingList<string>(data);

            foreach (string item in data)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine();

            list.ApplySort("a", System.ComponentModel.ListSortDirection.Ascending);
            foreach (string item in list)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine();

            list.RemoveSort();
            foreach (string item in list)
            {
                Console.WriteLine(item);
            }
        }
    }
}
