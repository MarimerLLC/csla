using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

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
        [TestMethod()]
        public void AscendingSort()
        {
            int[] intArr = { 45, 23, 56, 56, 11, 87, 94, 44 };
            SortedBindingList<int> sortedList = new SortedBindingList<int>(intArr);
            sortedList.ListChanged += new ListChangedEventHandler(sortedList_ListChanged);

            Assert.AreEqual(false, sortedList.IsSorted);
            Assert.AreEqual(56, intArr[2]);
            sortedList.ApplySort("", ListSortDirection.Ascending);
            Assert.AreEqual(44, sortedList[2]);
            Assert.AreEqual(8, sortedList.Count);
            Assert.AreEqual(true, sortedList.Contains(56));
            Assert.AreEqual(2, sortedList.IndexOf(56));
            Assert.AreEqual(true, sortedList.IsReadOnly);
            Assert.AreEqual(true, sortedList.IsSorted);

            foreach (int item in sortedList)
            {
                Console.WriteLine(item.ToString());
            }

            sortedList.RemoveSort();
            Assert.AreEqual(false, sortedList.IsSorted);
            Assert.AreEqual(56, sortedList[2]);
        }

        public void sortedList_ListChanged(object sender, ListChangedEventArgs e)
        {
            //called only on applysort() and removesort() 
            Console.WriteLine("list changed");
        }

        [TestMethod()]
        public void DescendingSort()
        {
            string[] strArr = { "zandy", "alex", "Chris", "bert", "alfred", "Bert", "Jimmy", "chris", "chris", "mobbit", "myper", "Corey", "Monkey" };
            SortedBindingList<string> sortedList = new SortedBindingList<string>(strArr);

            Assert.AreEqual("Bert", sortedList[5]);

            sortedList.ApplySort("", ListSortDirection.Descending);

            foreach (string item in sortedList)
            {
                Console.WriteLine(item);
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                Console.WriteLine("regular loop: " + sortedList[i]);
            }

            Assert.AreEqual("Corey", sortedList[5]);

            Console.WriteLine();
            Console.WriteLine(sortedList.Count);
        }

        [TestMethod()]
        public void CopyTo()
        { }

        [TestMethod()]
        public void Find()
        { }

    }
}
