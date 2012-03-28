using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class DashboardTest
  {
    [TestMethod]
    public void GetDashboard()
    {
      var obj = Dashboard.GetDashboard();
      Assert.AreEqual(1, obj.ProjectCount);
      Assert.AreEqual(1, obj.OpenProjectCount);
      Assert.AreEqual(1, obj.ResourceCount);
    }
  }
}
