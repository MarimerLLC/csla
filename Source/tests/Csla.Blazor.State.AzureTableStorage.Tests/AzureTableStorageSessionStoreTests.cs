//-----------------------------------------------------------------------
// <copyright file="AzureTableStorageSessionStoreTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Integration tests for the Azure Table Storage session store (require Azurite)</summary>
//-----------------------------------------------------------------------

using Azure.Data.Tables;
using FluentAssertions;
using Csla.Configuration;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.State.AzureTableStorage.Tests;

/// <summary>
/// Round-trip tests against a real Azure Table endpoint. They use the
/// well-known Azurite development connection string and are excluded from CI
/// because they require the emulator (or a live storage account) to be running.
/// Start Azurite locally (for example: <c>azurite</c>) before running them.
/// </summary>
/// <remarks>
/// A second store instance over the same table stands in for a restarted or
/// different server: it has no sessions in memory, so it reads the table.
/// </remarks>
[TestClass]
[TestCategory("SkipOnCIServer")]
public class AzureTableStorageSessionStoreTests
{
  private const string DevelopmentConnectionString = "UseDevelopmentStorage=true";
  private const string Discriminator = "test-app";

  private ServiceProvider _provider = null!;
  private TableServiceClient _serviceClient = null!;
  private FakeTimeProvider _timeProvider = null!;
  private string _tableName = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddCsla();
    _provider = services.BuildServiceProvider();

    // Session.Touch uses the system clock, so the fake clock starts at the real time.
    _timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

    // Unique table per test so runs do not collide. Table names must be alphanumeric.
    _tableName = "CslaTest" + Guid.NewGuid().ToString("N");
    _serviceClient = new TableServiceClient(DevelopmentConnectionString);
  }

  [TestCleanup]
  public void TestCleanup()
  {
    _serviceClient.DeleteTable(_tableName);
    _provider.Dispose();
  }

  private AzureTableStorageSessionStore CreateStore(string discriminator = Discriminator)
  {
    var options = new AzureTableStorageOptions
    {
      TableName = _tableName,
      ApplicationDiscriminator = discriminator,
    };
    return new AzureTableStorageSessionStore(
      _serviceClient,
      options,
      _provider.GetRequiredService<IServiceScopeFactory>(),
      _timeProvider,
      _provider.GetRequiredService<ILogger<AzureTableStorageSessionStore>>());
  }

  private async Task AddStaleEntityAsync(string key, string discriminator)
  {
    var entity = new SessionEntity
    {
      PartitionKey = key,
      RowKey = discriminator,
      Value = [0],
      LastTouched = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
    };
    await _serviceClient.GetTableClient(_tableName).UpsertEntityAsync(entity);
  }

  private async Task<bool> EntityExistsAsync(string key, string discriminator = Discriminator)
  {
    var response = await _serviceClient.GetTableClient(_tableName).GetEntityIfExistsAsync<SessionEntity>(key, discriminator);
    return response.HasValue;
  }

  [TestMethod]
  public async Task GetSessionAsync_MissingKey_ReturnsNull()
  {
    var store = CreateStore();

    var result = await store.GetSessionAsync("does-not-exist");

    result.Should().BeNull();
  }

  [TestMethod]
  public async Task GetSessionAsync_ReturnsTheStoredInstance()
  {
    var store = CreateStore();
    var session = new Session { ["v"] = 1 };
    await store.AddOrUpdateSessionAsync("key", session);

    var loaded = await store.GetSessionAsync("key");

    loaded.Should().BeSameAs(session);
  }

  [TestMethod]
  public async Task GetSessionAsync_ChangesMadeInPlace_AreVisibleToLaterReads()
  {
    var store = CreateStore();
    await store.AddOrUpdateSessionAsync("key", new Session());

    (await store.GetSessionAsync("key"))!["v"] = "changed";
    var loaded = await store.GetSessionAsync("key");

    loaded!["v"].Should().Be("changed");
  }

  [TestMethod]
  public async Task GetSessionAsync_NotInMemory_LoadsFromTable()
  {
    await CreateStore().AddOrUpdateSessionAsync("key", new Session { ["greeting"] = "hello" });
    var restarted = CreateStore();

    var first = await restarted.GetSessionAsync("key");
    var second = await restarted.GetSessionAsync("key");

    first.Should().NotBeNull();
    first!["greeting"].Should().Be("hello");
    second.Should().BeSameAs(first);
  }

  [TestMethod]
  public async Task AddOrUpdateSessionAsync_Twice_WritesLatestToTable()
  {
    var store = CreateStore();

    await store.AddOrUpdateSessionAsync("key", new Session { ["v"] = 1 });
    await store.AddOrUpdateSessionAsync("key", new Session { ["v"] = 2 });
    var loaded = await CreateStore().GetSessionAsync("key");

    loaded!["v"].Should().Be(2);
  }

  [TestMethod]
  public async Task SessionsWithSameKey_AreIsolatedByDiscriminator()
  {
    await CreateStore("app-a").AddOrUpdateSessionAsync("shared-key", new Session { ["app"] = "a" });
    await CreateStore("app-b").AddOrUpdateSessionAsync("shared-key", new Session { ["app"] = "b" });

    (await CreateStore("app-a").GetSessionAsync("shared-key"))!["app"].Should().Be("a");
    (await CreateStore("app-b").GetSessionAsync("shared-key"))!["app"].Should().Be("b");
  }

  [TestMethod]
  public async Task PurgeExpiredSessionsAsync_RemovesOnlyThisApplicationsStaleRows()
  {
    var store = CreateStore();
    await AddStaleEntityAsync("stale", Discriminator);
    await AddStaleEntityAsync("stale-other-app", "other-app");
    await store.AddOrUpdateSessionAsync("fresh", new Session { ["v"] = 1 });

    await store.PurgeExpiredSessionsAsync(TimeSpan.FromHours(1));

    (await EntityExistsAsync("stale")).Should().BeFalse();
    (await EntityExistsAsync("stale-other-app", "other-app")).Should().BeTrue();
    (await EntityExistsAsync("fresh")).Should().BeTrue();
  }

  [TestMethod]
  public async Task PurgeExpiredSessionsAsync_SavesChangesMadeInPlace()
  {
    var store = CreateStore();
    await store.AddOrUpdateSessionAsync("key", new Session { ["v"] = 1 });
    (await store.GetSessionAsync("key"))!["v"] = 2;

    await store.PurgeExpiredSessionsAsync(TimeSpan.FromHours(1));

    (await CreateStore().GetSessionAsync("key"))!["v"].Should().Be(2);
  }

  [TestMethod]
  public async Task PurgeExpiredSessionsAsync_RemovesExpiredSessionsFromMemoryAndTable()
  {
    var store = CreateStore();
    await store.AddOrUpdateSessionAsync("key", new Session { ["v"] = 1 });

    _timeProvider.Advance(TimeSpan.FromHours(2));
    await store.PurgeExpiredSessionsAsync(TimeSpan.FromHours(1));

    (await EntityExistsAsync("key")).Should().BeFalse();
    (await store.GetSessionAsync("key")).Should().BeNull();
  }

  [TestMethod]
  public async Task Store_ResolvedWithScopeValidation_HandlesConcurrentOperations()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddCsla(o => o.UseAzureTableStorageSessionStore(s =>
    {
      s.ConnectionString = DevelopmentConnectionString;
      s.TableName = _tableName;
      s.ApplicationDiscriminator = Discriminator;
    }));
    using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    var store = provider.GetRequiredService<ISessionStore>();
    var keys = Enumerable.Range(0, 20).Select(i => $"key-{i}").ToList();

    await Task.WhenAll(keys.Select(k => store.AddOrUpdateSessionAsync(k, new Session { ["k"] = k }).AsTask()));
    var restarted = CreateStore();
    var loaded = await Task.WhenAll(keys.Select(k => restarted.GetSessionAsync(k).AsTask()));

    loaded.Select(s => s!["k"]).Should().Equal(keys);
  }
}
