using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Csla.Properties;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.ApplicationModel.Resources.Core;

namespace Csla.WinRT.Test
{
    [TestClass]
    public class ResourcesTest
    {
      [TestInitialize]
      public void Initialize()
      {
        //Make sure to run tests with english resources 
        ResourceManager.Current.DefaultContext.Languages = new ReadOnlyCollection<string>(new List<string>() { "en" });
      }

      [TestMethod]
      public void GetClearInvalidExceptionReturnString()
      {
        var actual = Resources.ClearInvalidException;
        Assert.AreEqual("Clear is an invalid operation", actual);
      }

      [TestMethod]
      public void GetChangeInvalidExceptionReturnString()
      {
        var actual = Resources.ChangeInvalidException;
        Assert.AreEqual("Changing an element is an invalid operation", actual);
      }

      [TestMethod]
      public void GetConstructorsWithParametersNotSupportedString()
      {
        var actual = Resources.ConstructorsWithParametersNotSupported;
        Assert.AreEqual("Constructor with parameters are not supported", actual);
      }
    }
}
