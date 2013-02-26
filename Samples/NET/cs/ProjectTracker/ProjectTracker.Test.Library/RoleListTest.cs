using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace ProjectTracker.Test.Library
{
  /// <summary>
  /// Summary description for RoleListTest
  /// </summary>
  [TestClass]
  public class RoleListTest
  {
    [TestMethod]
    public void GetList()
    {
      var obj = RoleList.GetList();
      Assert.IsTrue(obj.Count > 0);
    }
  }
}
