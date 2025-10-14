using GraphMergerTest.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphMergerTest.BusinessTests
{
  [TestClass]
  public class TestBase
  {
    public static TestDIContext TestDIContext { get; private set; }

    public static Widget.Factory WidgetFactory { get; private set; }

    public static void TestBaseClassInitialize()
    {
      TestDIContext = TestDIContextFactory.CreateDefaultContext();

      WidgetFactory = TestDIContext.ServiceProvider.GetRequiredService<Widget.Factory>();
    }
  }
}