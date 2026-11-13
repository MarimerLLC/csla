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
using Csla.Rules;
using Microsoft.Extensions.DependencyInjection;

public static class MyExtensions
{
  public static IServiceCollection AddMine(this IServiceCollection services) => services;
}

public sealed class Customer : BusinessBase<Customer> { }

public sealed class Lookup : NameValueListBase<int, string> { }

public static class Startup
{
  public static async Task Configure(IServiceCollection services, Customer customer, IDataPortal<Customer> portal, HttpProxyOptions proxy, ApplicationContext context, Lookup lookup, IRuleContext ruleContext, IBusinessRuleBase rule)
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
    services?.AddCsla();
    Csla.Core.ObjectCloner.GetInstance(context);
    RuleUri.Parse(""csla://rule"");
    lookup.Clone();
    ruleContext.GetChainedContext(rule);
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
      Assert.IsTrue(diagnostics.IsSuppressedAt("services?.AddCsla()"), "conditional access");
      Assert.IsFalse(diagnostics.IsSuppressedAt("Csla.Core.ObjectCloner.GetInstance(context)"), "static factory method");
      Assert.IsFalse(diagnostics.IsSuppressedAt("RuleUri.Parse(\"csla://rule\")"), "static Parse method");
      // Same return type as the receiver, but a new object rather than fluent chaining.
      Assert.IsFalse(diagnostics.IsSuppressedAt("lookup.Clone()"), "Clone()");
      Assert.IsFalse(diagnostics.IsSuppressedAt("ruleContext.GetChainedContext(rule)"), "GetChainedContext()");
    }

    [TestMethod]
    public async Task DoNotSuppressInOtherAssembliesNamedCsla()
    {
      var code =
@"public sealed class Money
{
  public Money Round() => this;
}

public static class Usage
{
  public static void Run(Money money)
  {
    money.Round();
  }
}";
      var diagnostics = await TestHelpers.GetDiagnosticsWithSuppressorAsync(
        code, new ExpressionValueIsUnusedStubAnalyzer(), new FluentConfigurationSuppressor(), "Csla.Contrib");

      Assert.IsFalse(diagnostics.IsSuppressedAt("money.Round()"));
    }
  }
}
