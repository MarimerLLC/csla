//-----------------------------------------------------------------------
// <copyright file="SortedBindingListTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
            int[] intArr = { 45, 23, 57, 56, 11, 87, 94, 44 };
            SortedBindingList<int> sortedList = new SortedBindingList<int>(intArr);
            sortedList.ListChanged += new ListChangedEventHandler(sortedList_ListChanged);

            Assert.AreEqual(false, sortedList.IsSorted);
            Assert.AreEqual(56, intArr[3]);
            sortedList.ApplySort("", ListSortDirection.Ascending);
            Assert.AreEqual(44, sortedList[2]);
            Assert.AreEqual(8, sortedList.Count);
            Assert.AreEqual(true, sortedList.Contains(56));
            Assert.AreEqual(4, sortedList.IndexOf(56));
            Assert.AreEqual(true, sortedList.IsReadOnly);
            Assert.AreEqual(true, sortedList.IsSorted);

            foreach (int item in sortedList)
            {
                Console.WriteLine(item.ToString());
            }

            sortedList.RemoveSort();
            Assert.AreEqual(false, sortedList.IsSorted);
            Assert.AreEqual(56, sortedList[3]);
            
            // This list dowes not support searching 
            Assert.IsFalse(sortedList.SupportsSearching);
            // and Find should return -1 because underlying list dows not implement IBindingList
            Assert.AreEqual(-1, sortedList.Find("", 56));
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

        //[TestMethod()]
        //public void TestFirstItemIssue()
        //{
        //    int[] intArray = { 10, 4, 6 };
        //    SortedBindingList<int> sortedList = new SortedBindingList<int>(intArray);

        //    foreach (int item in sortedList)
        //    {
        //        Console.WriteLine(item.ToString());
        //    }

        //    sortedList.ApplySort("", ListSortDirection.Ascending);

        //    Console.WriteLine();

        //    foreach (int item in sortedList)
        //    {
        //        Console.WriteLine(item.ToString());
        //    }

        //    Console.WriteLine();

        //    sortedList.ApplySort("", ListSortDirection.Descending);

        //    foreach (int item in sortedList)
        //    {
        //        Console.WriteLine(item.ToString());
        //    }
        //}

        [TestMethod()]
        public void CopyTo()
        {
            int[] intArray = { 5, 7, 1, 3, 5, 44, 32 };
            SortedBindingList<int> sortedList = new SortedBindingList<int>(intArray);

            int[] intArray2 = { 3, 75, 1222, 3333, 511, 443, 332 };

            Assert.AreEqual(1222, intArray2[2]);

            sortedList.ApplySort("", ListSortDirection.Descending);
            Assert.AreEqual(44, sortedList[0], "Sorted values incorrect");

            sortedList.CopyTo(intArray2, 0);

            Assert.AreEqual(44, intArray2[0], "Copied values incorrect");
            Assert.AreEqual(7, intArray2[2], "Copied values incorrect");

            foreach (int item in intArray2)
            {
                Console.WriteLine(item.ToString());
            }        
        }

      [TestMethod]
      public void IndexOf()
      {
        List<string> list = new List<string>();

        string barney = "Barney";
        string charlie = "Charlie";
        string zeke = "Zeke";

        list.AddRange(new string[] { charlie, barney, zeke });

        SortedBindingList<string> sortedList = new SortedBindingList<string>(list);

        Assert.AreEqual(1, sortedList.IndexOf(barney), "Unsorted index should be 1");

        sortedList.ApplySort(string.Empty, System.ComponentModel.ListSortDirection.Ascending);

        Assert.AreEqual(1, sortedList.IndexOf(charlie), "Sorted index should be 1");
      }

      [TestMethod]
      public void SourceList()
      {
        List<string> list = new List<string>();

        string barney = "Barney";
        string charlie = "Charlie";
        string zeke = "Zeke";

        list.AddRange(new string[] { charlie, barney, zeke });

        SortedBindingList<string> sortedList = new SortedBindingList<string>(list);

        Assert.IsTrue(ReferenceEquals(list, sortedList.SourceList), "List references should match");
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void ApplySort_ThrowException_WhenPropertyNameNotFound()
      {
        int[] intArray = { 5, 7, 1, 3, 5, 44, 32 };
        SortedBindingList<int> sortedList = new SortedBindingList<int>(intArray);
        sortedList.ApplySort("NotAProperty", ListSortDirection.Ascending);
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void Find_ThrowException_WhenPropertyNameNotFound()
      {
        int[] intArray = { 5, 7, 1, 3, 5, 44, 32 };
        SortedBindingList<int> sortedList = new SortedBindingList<int>(intArray);
        sortedList.Find("NotAProperty", 7);
      }
    }
}