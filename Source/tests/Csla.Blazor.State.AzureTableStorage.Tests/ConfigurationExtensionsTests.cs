//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensionsTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CI-safe DI registration tests for the Azure Table Storage session store</summary>
//-----------------------------------------------------------------------

using Azure.Data.Tables;
using FluentAssertions;
using Csla.Configuration;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.State.AzureTableStorage.Tests;

/// <summary>
/// Verifies the registrations made by <see cref="AzureTableStorageConfigurationExtensions"/>.
/// No test resolves the session store itself, because its constructor contacts
/// Azure Storage; creating a <see cref="TableServiceClient"/> does not.
/// </summary>
[TestClass]
public class ConfigurationExtensionsTests
{
  private const string DevelopmentConnectionString = "UseDevelopmentStorage=true";

  [TestMethod]
  public void UseAzureTableStorageSessionStore_RegistersSingletonISessionStore()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).UseAzureTableStorageSessionStore(o => o.ConnectionString = DevelopmentConnectionString);

    var descriptor = services.Single(d => d.ServiceType == typeof(ISessionStore));
    descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    descriptor.ImplementationFactory.Should().NotBeNull();
  }

  [TestMethod]
  public void UseAzureTableStorageSessionStore_AfterAddServerSideBlazor_IsLastISessionStoreRegistration()
  {
    var services = new ServiceCollection();

    services.AddCsla(o => o
      .AddServerSideBlazor(b => b.UseInMemoryApplicationContextManager = false)
      .UseAzureTableStorageSessionStore(s => s.ConnectionString = DevelopmentConnectionString));

    var descriptor = services.Last(d => d.ServiceType == typeof(ISessionStore));
    descriptor.ImplementationType.Should().BeNull();
    descriptor.ImplementationFactory.Should().NotBeNull();
  }

  [TestMethod]
  public void UseAzureTableStorageSessionStore_RegistersConfiguredOptionsAsSingleton()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).UseAzureTableStorageSessionStore(o =>
    {
      o.ConnectionString = DevelopmentConnectionString;
      o.TableName = "CustomTable";
    });

    var descriptor = services.Single(d => d.ServiceType == typeof(AzureTableStorageOptions));
    descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    ((AzureTableStorageOptions)descriptor.ImplementationInstance!).TableName.Should().Be("CustomTable");
  }

  [TestMethod]
  public void UseAzureTableStorageSessionStore_KeepsRegisteredTimeProvider()
  {
    var services = new ServiceCollection();
    var timeProvider = new FakeTimeProvider();
    services.AddSingleton<TimeProvider>(timeProvider);

    new CslaOptions(services).UseAzureTableStorageSessionStore(o => o.ConnectionString = DevelopmentConnectionString);

    services.Single(d => d.ServiceType == typeof(TimeProvider)).ImplementationInstance.Should().BeSameAs(timeProvider);
  }

  [TestMethod]
  public void UseAzureTableStorageSessionStore_DoesNotRegisterTableServiceClient()
  {
    var services = new ServiceCollection();

    new CslaOptions(services).UseAzureTableStorageSessionStore(o => o.ConnectionString = DevelopmentConnectionString);

    services.Should().NotContain(d => d.ServiceType == typeof(TableServiceClient));
  }

  [TestMethod]
  public void UseAzureTableStorageSessionStore_ReturnsSameConfigInstance()
  {
    var config = new CslaOptions(new ServiceCollection());

    var result = config.UseAzureTableStorageSessionStore(o => o.ConnectionString = DevelopmentConnectionString);

    result.Should().BeSameAs(config);
  }

  [TestMethod]
  public void UseAzureTableStorageSessionStore_NullConfig_Throws()
  {
    CslaOptions config = null!;

    var act = () => config.UseAzureTableStorageSessionStore();

    act.Should().Throw<ArgumentNullException>();
  }

  [TestMethod]
  public void GetTableServiceClient_ConnectionString_IgnoresRegisteredClient()
  {
    var registered = new TableServiceClient(new Uri("https://registered.table.core.windows.net"), new TableSharedKeyCredential("registered", Convert.ToBase64String(new byte[32])));
    var provider = new ServiceCollection()
      .AddSingleton(registered)
      .BuildServiceProvider();
    var options = new AzureTableStorageOptions { ConnectionString = DevelopmentConnectionString };

    var client = AzureTableStorageConfigurationExtensions.GetTableServiceClient(provider, options);

    client.Should().NotBeSameAs(registered);
    client.AccountName.Should().Be("devstoreaccount1");
  }

  [TestMethod]
  public void GetTableServiceClient_ServiceKey_ResolvesKeyedClient()
  {
    var keyed = new TableServiceClient(DevelopmentConnectionString);
    var provider = new ServiceCollection()
      .AddKeyedSingleton("state", keyed)
      .AddSingleton(new TableServiceClient(DevelopmentConnectionString))
      .BuildServiceProvider();
    var options = new AzureTableStorageOptions { ServiceKey = "state" };

    var client = AzureTableStorageConfigurationExtensions.GetTableServiceClient(provider, options);

    client.Should().BeSameAs(keyed);
  }

  [TestMethod]
  public void GetTableServiceClient_NoConnectionStringOrKey_ResolvesDefaultClient()
  {
    var registered = new TableServiceClient(DevelopmentConnectionString);
    var provider = new ServiceCollection()
      .AddSingleton(registered)
      .BuildServiceProvider();

    var client = AzureTableStorageConfigurationExtensions.GetTableServiceClient(provider, new AzureTableStorageOptions());

    client.Should().BeSameAs(registered);
  }
}
