using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ResourceListTest
  {
    [TestMethod]
    public void GetFullList()
    {
      var obj = ResourceList.GetResourceList();
      Assert.IsTrue(obj.Count > 0);
    }
  }
}
