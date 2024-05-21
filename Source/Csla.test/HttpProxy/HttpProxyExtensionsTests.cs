using Csla.Channels.Http;
using Csla.Configuration;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.HttpProxy;

[TestClass]
public class HttpProxyExtensionsTests
{
  [TestMethod]
  public void UseHttpProxy_WhenConfiguringACustomHttpClientFactoryItShouldBeUsedWhenHttpProxyIsCreated()
  {
    bool hasBeenCalled = false;
    HttpClient CustomFactory(IServiceProvider sp)
    {
      hasBeenCalled = true;
      return new HttpClient();
    }

    var diContext = TestDIContextFactory.CreateContext(
      o => o.DataPortal(
        dp => dp.ClientSideDataPortal(
          cdp => cdp.UseHttpProxy(
            hp => hp.WithHttpClientFactory(CustomFactory)
            )
          )
        )
      );

    _ = diContext.ServiceProvider.GetRequiredService<DataPortalClient.IDataPortalProxy>();

    hasBeenCalled.Should().BeTrue();
  }
}