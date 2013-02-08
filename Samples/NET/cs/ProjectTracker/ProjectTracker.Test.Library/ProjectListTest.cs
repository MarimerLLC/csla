using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ProjectListTest
  {
    [TestMethod]
    public void GetFullList()
    {
      var obj = ProjectList.GetProjectList();
      Assert.IsTrue(obj.Count > 0);
    }

    [TestMethod]
    public void GetFilteredList()
    {
      var obj = ProjectList.GetProjectList("Tracker");
      Assert.IsTrue(obj.Count > 0);
    }

    [TestMethod]
    public void GetEmptyFilteredList()
    {
      var obj = ProjectList.GetProjectList("xyz");
      Assert.AreEqual(0, obj.Count);
    }
  }
}
