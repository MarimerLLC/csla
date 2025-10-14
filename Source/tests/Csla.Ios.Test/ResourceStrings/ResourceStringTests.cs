using NUnit.Framework;
using System;

namespace Csla.Ios.Test.ResourceStrings
{
  [TestFixture]
  public class ResourceStringTests
  {
    [Test]
    public void TestResourceStringsShouldNotThrow()
    {
      try
      {
        var result = Csla.Properties.Resources.MaxValueRule;
      }
      catch (Exception ex)
      {
        Assert.Fail(string.Format("{0}: {1}", ex.GetType().Name, ex.Message));
      }
    }
  }
}
