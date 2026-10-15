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
using Csla.Serialization;
using Csla.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.State.AzureTableStorage.Tests;

/// <summary>
/// Round-trip tests against a real Azure Table endpoint. They use the
/// well-known Azurite development connection string and are excluded from CI
/// because they require the emulator (or a live storage account) to be running.
/// Start Azurite locally (for example: <c>azurite</c>) before running them.
/// </summary>
[TestClass]
[TestCategory("SkipOnCIServer")]
public class AzureTableStorageSessionStoreTests
{
  private const string DevelopmentConnectionString = "UseDevelopmentStorage=true";
  private const string Discriminator = "test-app";

  private ServiceProvider _provider = null!;
  private TableServiceClient _serviceClient = null!;
  private string _tableName = null!;

  [TestInitialize]
  public void TestInitialize()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddCsla();
    _provider = services.BuildServiceProvider();

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
      _provider.GetRequiredService<ISerializationFormatter>(),
      options,
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

  private async Task<bool> EntityExistsAsync(string key, string discriminator)
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
  public async Task AddOrUpdateSessionAsync_ThenGet_RoundTripsValues()
  {
    var store = CreateStore();
    var session = new Session { ["greeting"] = "hello" };

    await store.AddOrUpdateSessionAsync("key-1", session);
    var loaded = await store.GetSessionAsync("key-1");

    loaded.Should().NotBeNull();
    loaded!["greeting"].Should().Be("hello");
    loaded.LastTouched.Should().Be(session.LastTouched);
  }

  [TestMethod]
  public async Task AddOrUpdateSessionAsync_Twice_Upserts()
  {
    var store = CreateStore();

    await store.AddOrUpdateSessionAsync("key-2", new Session { ["v"] = 1 });
    await store.AddOrUpdateSessionAsync("key-2", new Session { ["v"] = 2 });
    var loaded = await store.GetSessionAsync("key-2");

    loaded.Should().NotBeNull();
    loaded!["v"].Should().Be(2);
  }

  [TestMethod]
  public async Task SessionsWithSameKey_AreIsolatedByDiscriminator()
  {
    var storeA = CreateStore("app-a");
    var storeB = CreateStore("app-b");

    await storeA.AddOrUpdateSessionAsync("shared-key", new Session { ["app"] = "a" });
    await storeB.AddOrUpdateSessionAsync("shared-key", new Session { ["app"] = "b" });

    (await storeA.GetSessionAsync("shared-key"))!["app"].Should().Be("a");
    (await storeB.GetSessionAsync("shared-key"))!["app"].Should().Be("b");
  }

  [TestMethod]
  public async Task PurgeExpiredSessionsAsync_RemovesOnlyThisApplicationsStaleSessions()
  {
    var store = CreateStore();
    await AddStaleEntityAsync("stale", Discriminator);
    await AddStaleEntityAsync("stale-other-app", "other-app");
    await store.AddOrUpdateSessionAsync("fresh", new Session { ["v"] = 1 });

    await store.PurgeExpiredSessionsAsync(TimeSpan.FromHours(1));

    (await EntityExistsAsync("stale", Discriminator)).Should().BeFalse();
    (await EntityExistsAsync("stale-other-app", "other-app")).Should().BeTrue();
    (await EntityExistsAsync("fresh", Discriminator)).Should().BeTrue();
  }
}
