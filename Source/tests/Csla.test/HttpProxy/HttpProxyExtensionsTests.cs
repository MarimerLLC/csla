using Csla.Channels.Http;
using Csla.Configuration;
using Csla.Testing;
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

    using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(o => o.DataPortal(
        dp => dp.AddClientSideDataPortal(
          cdp => cdp.UseHttpProxy(
            hp => hp.WithHttpClientFactory(CustomFactory)
            )
          )
        )));

    _ = testHost.Services.GetRequiredService<DataPortalClient.IDataPortalProxy>();

    hasBeenCalled.Should().BeTrue();
  }
}