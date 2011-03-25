using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;
using System.Threading;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ProjectEditTest
  {
    [TestMethod]
    public void GetProject()
    {
      var obj = ProjectEdit.GetProject(1);
      Assert.IsNotNull(obj);
      Assert.AreEqual(1, obj.Id);
    }

    [TestMethod]
    public void GetProjectAsync()
    {
      var sync = new AutoResetEvent(false);
      ProjectEdit.GetProject(1, (o, e) =>
        {
          if (e.Error != null)
            Assert.Fail(e.Error.Message);
          var obj = e.Object;
          Assert.IsNotNull(obj);
          Assert.AreEqual(1, obj.Id);
          sync.Set();
        });
      sync.WaitOne(1000);
    }
  }
}
