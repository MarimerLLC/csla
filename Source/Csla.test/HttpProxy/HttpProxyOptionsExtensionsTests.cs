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
}
