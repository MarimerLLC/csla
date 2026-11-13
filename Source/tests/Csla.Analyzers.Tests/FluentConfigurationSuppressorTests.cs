using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests
{
  [TestClass]
  public sealed class FluentConfigurationSuppressorTests
  {
    [TestMethod]
    public void VerifySupportedSuppressions()
    {
      var suppressions = new FluentConfigurationSuppressor().SupportedSuppressions;
      Assert.AreEqual(1, suppressions.Length);

      Assert.AreEqual(Constants.SuppressorIdentifiers.FluentConfigurationResultUnused, suppressions[0].Id);
      Assert.AreEqual("IDE0058", suppressions[0].SuppressedDiagnosticId);
      Assert.IsFalse(string.IsNullOrWhiteSpace(suppressions[0].Justification.ToString()));
    }

    [TestMethod]
    public async Task SuppressOnlyCslaFluentMethods()
    {
      var code =
@"using System;
using System.Threading.Tasks;
using Csla;
using Csla.Channels.Http;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class MyExtensions
{
  public static IServiceCollection AddMine(this IServiceCollection services) => services;
}

public sealed class Customer : BusinessBase<Customer> { }

public static class Startup
{
  public static async Task Configure(IServiceCollection services, Customer customer, IDataPortal<Customer> portal, HttpProxyOptions proxy)
  {
    services.AddCsla();
    services.AddCsla(o => o.Rules(r => { r.UseBatchCheckRules(); }));
    ConfigurationExtensions.AddCsla(services);
    proxy.WithDataPortalUrl(""https://localhost/api/dataportal"").UseTextForSerialization();
    (services.AddCsla()).AddCsla();
    services.AddMine();
    customer.Save();
    await customer.SaveAsync();
    portal.Fetch(1);
    await portal.FetchAsync(1);
    customer.ToString();
  }
}";
      var diagnostics = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        code, new ExpressionValueIsUnusedStubAnalyzer(), new FluentConfigurationSuppressor());

      Assert.IsTrue(diagnostics.IsSuppressedAt("services.AddCsla()"), "AddCsla()");
      Assert.IsTrue(diagnostics.IsSuppressedAt("services.AddCsla(o => o.Rules(r => { r.UseBatchCheckRules(); }))"), "AddCsla(options)");
      Assert.IsTrue(diagnostics.IsSuppressedAt("r.UseBatchCheckRules()"), "UseBatchCheckRules (instance method)");
      Assert.IsTrue(diagnostics.IsSuppressedAt("ConfigurationExtensions.AddCsla(services)"), "AddCsla called as a static method");
      Assert.IsTrue(diagnostics.IsSuppressedAt(@"proxy.WithDataPortalUrl(""https://localhost/api/dataportal"").UseTextForSerialization()"), "chained calls");
      Assert.IsTrue(diagnostics.IsSuppressedAt("(services.AddCsla()).AddCsla()"), "parenthesized receiver");

      Assert.IsFalse(diagnostics.IsSuppressedAt("services.AddMine()"), "non-CSLA fluent method");
      Assert.IsFalse(diagnostics.IsSuppressedAt("customer.Save()"), "Save()");
      Assert.IsFalse(diagnostics.IsSuppressedAt("await customer.SaveAsync()"), "SaveAsync()");
      Assert.IsFalse(diagnostics.IsSuppressedAt("portal.Fetch(1)"), "Fetch()");
      Assert.IsFalse(diagnostics.IsSuppressedAt("await portal.FetchAsync(1)"), "FetchAsync()");
      Assert.IsFalse(diagnostics.IsSuppressedAt("customer.ToString()"), "ToString()");
    }
  }
}
