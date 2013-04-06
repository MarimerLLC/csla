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
  public class ProjectCloserTest
  {
    [TestInitialize]
    public void Setup()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;
    }

    [TestMethod]
    public void CloseProject()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj.Started = DateTime.Today.Subtract(new TimeSpan(5, 0, 0));
      obj = obj.Save();

      var result = ProjectCloser.CloseProject(obj.Id);
      Assert.IsTrue(result.Closed);

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    public void CloseProjectAsync()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj.Started = DateTime.Today.Subtract(new TimeSpan(5, 0, 0));
      obj = obj.Save();

      var sync = new AutoResetEvent(false);
      ProjectCloser.CloseProject(obj.Id, (o, e) =>
        {
          if (e.Error != null)
            Assert.Fail(e.Error.Message);
          var result = e.Object;
          Assert.IsTrue(result.Closed);
        });
      sync.WaitOne(1000);

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CloseProjectFail()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj.Started = DateTime.Today.Subtract(new TimeSpan(5, 0, 0));
      obj.Ended = DateTime.Today;
      obj = obj.Save();

      try
      {
        var result = ProjectCloser.CloseProject(obj.Id);
      }
      catch (Csla.DataPortalException ex)
      {
        throw ex.BusinessException;
      }
      finally
      {
        ProjectEdit.DeleteProject(obj.Id);
      }
    }
  }
}
