using Csla.Channels.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.HttpProxy;

[TestClass]
public class HttpProxyOptionsExtensionsTests
{
  [TestMethod]
  public void WithDataPortalUrl_ShouldSetTheDataPortalUrlAsExpected()
  {
    var options = new HttpProxyOptions();
    options.WithDataPortalUrl("http://localhost:1337");

    var expectedOptions = new HttpProxyOptions
    {
      DataPortalUrl = "http://localhost:1337"
    };

    options.Should().BeEquivalentTo(expectedOptions);
  }

  [TestMethod]
  public void UseTextForSerialization_ShouldSetTheUseTextSerializationToTrue()
  {
    var options = new HttpProxyOptions();
    options.UseTextForSerialization();

    var expectedOptions = new HttpProxyOptions
    {
      UseTextSerialization = true
    };

    options.Should().BeEquivalentTo(expectedOptions);
  }

  [TestMethod]
  public void UseDefaultForSerialization_ShouldSetTheUseTextSerializationToFalse()
  {
    var options = new HttpProxyOptions();
    options.UseDefaultForSerialization();

    var expectedOptions = new HttpProxyOptions
    {
      UseTextSerialization = false
    };

    options.Should().BeEquivalentTo(expectedOptions);
  }

  [TestMethod]
  public void WithHttpClientFactory_ShouldSetTheFactoryToTheProvidedFactory()
  {
    var options = new HttpProxyOptions();
    options.WithHttpClientFactory(httpClientFactory);

    var expectedOptions = new HttpProxyOptions
    {
      HttpClientFactory = httpClientFactory
    };

    options.Should().BeEquivalentTo(expectedOptions);

    static HttpClient httpClientFactory(IServiceProvider sp) => new();
  }

  [TestMethod]
  public void WithTimeout_ShouldSetTheTimeoutAsExpected()
  {
    var options = new HttpProxyOptions();
    var timeout = TimeSpan.FromSeconds(30);
    options.WithTimeout(timeout);

    var expectedOptions = new HttpProxyOptions
    {
      Timeout = timeout
    };

    options.Should().BeEquivalentTo(expectedOptions);
  }

  [TestMethod]
  public void WithReadWriteTimeout_ShouldSetTheReadWriteTimeoutAsExpected()
  {
    var options = new HttpProxyOptions();
    var readWriteTimeout = TimeSpan.FromSeconds(60);
    options.WithReadWriteTimeout(readWriteTimeout);

    var expectedOptions = new HttpProxyOptions
    {
      ReadWriteTimeout = readWriteTimeout
    };

    options.Should().BeEquivalentTo(expectedOptions);
  }
}
