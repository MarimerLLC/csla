using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Validation.Test
{
  [TestClass]
  public class ValidationTest
  {
    [TestInitialize]
    public void Initialize()
    {
      Thread.CurrentPrincipal = new VPrincipal("ProjectManager");
    }

    [TestCleanup]
    public void Cleanup()
    {
      Thread.CurrentPrincipal = ClaimsPrincipal.Current;
    }

    [TestMethod]
    
    public void DateComparisonIsCorrect()
    {
      var project = DataPortal.Create<Project>();
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
      project.Ended = DateTime.Now.ToString();
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
      project.Started = DateTime.Now.AddDays(2).ToString();
      Assert.IsTrue(project.BrokenRulesCollection.Count > 0);
      Assert.AreEqual("Start date can't be after end date", ((IDataErrorInfo)project)["Started"]);
      Assert.AreEqual("Start date can't be after end date", ((IDataErrorInfo)project)["Ended"]);
    }


    [TestMethod]
    
    public void StringRequiredIsCorrect()
    {
      var project = DataPortal.Create<Project>();
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
      project.Name = "Test";
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
      project.Name = "";
      Assert.IsFalse(project.BrokenRulesCollection.Count == 0);
      project.Name = "Test";
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
    }

    [TestMethod]
    public void StringMaxLengthIsCorrect()
    {
      var project = DataPortal.Create<Project>();
      project.Name = "Test";
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
      project.Name = "Test with a much to long string value should cause a broken rule in business object ";
      Assert.IsFalse(project.BrokenRulesCollection.Count == 0);
      project.Name = "Test";
      Assert.IsTrue(project.BrokenRulesCollection.Count == 0);
    }
  }
}
