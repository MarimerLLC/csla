//-----------------------------------------------------------------------
// <copyright file="SessionManagerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for server-side SessionManager</summary>
//-----------------------------------------------------------------------

using Csla.Blazor.State;
using Csla.State;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.State;

[TestClass]
public class ServerSessionManagerTests
{
  private FakeSessionIdManager _sessionIdManager = null!;
  private InMemorySessionStore _sessionStore = null!;
  private SessionManager _sessionManager = null!;

  [TestInitialize]
  public void Initialize()
  {
    _sessionIdManager = new FakeSessionIdManager("test-session-id");
    _sessionStore = new InMemorySessionStore();
    _sessionManager = new SessionManager(_sessionIdManager, _sessionStore);
  }

  [TestMethod]
  public void GetSession_CreatesNewSessionWhenNotInStore()
  {
    var session = _sessionManager.GetSession();

    session.Should().NotBeNull();
  }

  [TestMethod]
  public void GetSession_ReturnsSameSessionOnSubsequentCalls()
  {
    var session1 = _sessionManager.GetSession();
    var session2 = _sessionManager.GetSession();

    session2.Should().BeSameAs(session1);
  }

  [TestMethod]
  public void GetSession_TouchesSession()
  {
    var session = _sessionManager.GetSession();

    session.LastTouched.Should().BeGreaterThan(0);
  }

  [TestMethod]
  public void UpdateSession_StoresNewSession()
  {
    var newSession = new Session();
    newSession["testKey"] = "testValue";

    _sessionManager.UpdateSession(newSession);

    var retrieved = _sessionManager.GetSession();
    retrieved.Should().BeSameAs(newSession);
  }

  [TestMethod]
  public void UpdateSession_ThrowsOnNull()
  {
    var act = () => _sessionManager.UpdateSession(null!);

    act.Should().Throw<ArgumentNullException>();
  }

  [TestMethod]
  public void PurgeSessions_DelegatesToStore()
  {
    // Create a session and let it age
    _sessionManager.GetSession();

    // Should not throw
    _sessionManager.PurgeSessions(TimeSpan.FromHours(1));
  }

  private class FakeSessionIdManager(string sessionId) : ISessionIdManager
  {
    public string GetSessionId() => sessionId;
  }
}
