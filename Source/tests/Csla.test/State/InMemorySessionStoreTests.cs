//-----------------------------------------------------------------------
// <copyright file="InMemorySessionStoreTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for InMemorySessionStore</summary>
//-----------------------------------------------------------------------

using Csla.Blazor.State;
using Csla.State;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.State;

[TestClass]
public class InMemorySessionStoreTests
{
  [TestMethod]
  public async Task GetSessionAsync_NonExistentKey_ReturnsNull()
  {
    var store = new InMemorySessionStore();

    var result = await store.GetSessionAsync("nonexistent");

    result.Should().BeNull();
  }

  [TestMethod]
  public async Task AddOrUpdateSessionAsync_NewKey_AddsSession()
  {
    var store = new InMemorySessionStore();
    var session = new Session();

    await store.AddOrUpdateSessionAsync("key1", session);

    var result = await store.GetSessionAsync("key1");
    result.Should().BeSameAs(session);
  }

  [TestMethod]
  public async Task AddOrUpdateSessionAsync_ExistingKey_ReplacesSession()
  {
    var store = new InMemorySessionStore();
    var session1 = new Session();
    var session2 = new Session();
    await store.AddOrUpdateSessionAsync("key1", session1);

    await store.AddOrUpdateSessionAsync("key1", session2);

    var result = await store.GetSessionAsync("key1");
    result.Should().BeSameAs(session2);
  }

  [TestMethod]
  public async Task PurgeExpiredSessionsAsync_RemovesExpiredSessions()
  {
    var store = new InMemorySessionStore();
    var expiredSession = new Session();
    await store.AddOrUpdateSessionAsync("expired", expiredSession);

    // Wait so the session ages past the 1-second expiration.
    // LastTouched uses Unix seconds, so we need > 2 seconds
    // to guarantee the strict less-than check passes.
    await Task.Delay(2100);

    var activeSession = new Session();
    activeSession.Touch();
    await store.AddOrUpdateSessionAsync("active", activeSession);

    await store.PurgeExpiredSessionsAsync(TimeSpan.FromSeconds(1));

    var expired = await store.GetSessionAsync("expired");
    expired.Should().BeNull();

    var active = await store.GetSessionAsync("active");
    active.Should().NotBeNull();
  }

  [TestMethod]
  public async Task PurgeExpiredSessionsAsync_RetainsActiveSessions()
  {
    var store = new InMemorySessionStore();
    var session = new Session();
    session.Touch();
    await store.AddOrUpdateSessionAsync("active", session);

    await store.PurgeExpiredSessionsAsync(TimeSpan.FromHours(1));

    var result = await store.GetSessionAsync("active");
    result.Should().NotBeNull();
  }
}
