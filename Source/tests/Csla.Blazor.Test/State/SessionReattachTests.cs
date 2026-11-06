//-----------------------------------------------------------------------
// <copyright file="SessionReattachTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests that restored session objects use the caller's ApplicationContext</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.AspNetCore.Blazor;
using Csla.Blazor.State;
using Csla.Blazor.Test.Fakes;
using Csla.Configuration;
using Csla.Core;
using Csla.Serialization;
using Csla.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.Test.State;

/// <summary>
/// Business objects restored from serialized session data are
/// created in a short-lived scope (a StateController request or
/// a session store's own scope). They must be usable from the
/// scope that later reads them.
/// </summary>
[TestClass]
public class SessionReattachTests
{
  private const string DirectKey = "direct";
  private const string LocalKey = "person";

  [TestMethod]
  public async Task LocalContextObjectRestoredFromSerializedSessionCanBeSaved()
  {
    using var provider = BuildProvider();
    StoreRestoredSession(provider);

    using var scope = CreateCircuitScope(provider);
    var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

    var person = (FakePerson)applicationContext.LocalContext[LocalKey];

    Assert.AreSame(applicationContext, ContextOf(person));
    Assert.AreSame(applicationContext, ContextOf(person.EmailAddresses[0]));
    person = await person.SaveAsync();
    Assert.IsFalse(person.IsNew);
  }

  [TestMethod]
  public async Task SessionObjectRestoredFromSerializedSessionCanBeSavedAfterInitialize()
  {
    using var provider = BuildProvider();
    StoreRestoredSession(provider);

    using var scope = CreateCircuitScope(provider);
    var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    var stateManager = scope.ServiceProvider.GetRequiredService<StateManager>();

    await stateManager.InitializeAsync();
    var person = (FakePerson)scope.ServiceProvider.GetRequiredService<ISessionManager>().GetSession()[DirectKey];

    Assert.AreSame(applicationContext, ContextOf(person));
    Assert.AreSame(applicationContext, ContextOf(person.EmailAddresses[0]));
    person = await person.SaveAsync();
    Assert.IsFalse(person.IsNew);
  }

  [TestMethod]
  public async Task SessionObjectFromEndedCircuitCanBeSavedInNewCircuit()
  {
    using var provider = BuildProvider();
    using (var firstCircuit = CreateCircuitScope(provider))
    {
      var applicationContext = firstCircuit.ServiceProvider.GetRequiredService<ApplicationContext>();
      applicationContext.LocalContext[LocalKey] = await CreatePersonAsync(firstCircuit.ServiceProvider, "First");
    }

    using var secondCircuit = CreateCircuitScope(provider);
    var secondContext = secondCircuit.ServiceProvider.GetRequiredService<ApplicationContext>();
    var person = (FakePerson)secondContext.LocalContext[LocalKey];

    Assert.AreSame(secondContext, ContextOf(person));
    person = await person.SaveAsync();
    Assert.IsFalse(person.IsNew);
  }

  [TestMethod]
  public async Task NonCloningSaveOfSessionObjectRunsInDataPortalScope()
  {
    using var provider = BuildProvider(autoCloneOnUpdate: false);
    using var scope = CreateCircuitScope(provider);
    var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    var recorder = await scope.ServiceProvider.GetRequiredService<IDataPortal<ScopeRecorder>>().CreateAsync();
    recorder.Name = "Recorder";
    applicationContext.LocalContext[LocalKey] = recorder;

    // LocalProxy reads LocalContext during the operation, which
    // must not pull the object back out of the data portal's scope
    recorder = await recorder.SaveAsync();

    Assert.IsTrue(recorder.InsertUsedDataPortalScope);
    var stored = (ScopeRecorder)applicationContext.LocalContext[LocalKey];
    Assert.AreSame(applicationContext, ContextOf(stored));
  }

  /// <summary>
  /// Puts business objects in the session (directly and in LocalContext),
  /// then replaces the session with a copy deserialized in a scope that
  /// is disposed, like StateController.Put or a persistent session store.
  /// </summary>
  private static void StoreRestoredSession(IServiceProvider provider)
  {
    byte[] data;
    using (var scope = CreateCircuitScope(provider))
    {
      var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
      applicationContext.LocalContext[LocalKey] = CreatePersonAsync(scope.ServiceProvider, "Local").GetAwaiter().GetResult();
      var session = scope.ServiceProvider.GetRequiredService<ISessionManager>().GetSession();
      session[DirectKey] = CreatePersonAsync(scope.ServiceProvider, "Direct").GetAwaiter().GetResult();
      data = scope.ServiceProvider.GetRequiredService<ISerializationFormatter>().Serialize(session);
    }

    Session restored;
    using (var deserializationScope = provider.CreateScope())
    {
      restored = (Session)deserializationScope.ServiceProvider.GetRequiredService<ISerializationFormatter>().Deserialize(data);
    }

    var restoredPerson = (FakePerson)restored[DirectKey];
    Assert.ThrowsExactly<ObjectDisposedException>(() => ContextOf(restoredPerson).GetRequiredService<IDataPortal<FakePerson>>());

    provider.GetRequiredService<ISessionManager>().UpdateSession(restored);
  }

  private static async Task<FakePerson> CreatePersonAsync(IServiceProvider serviceProvider, string firstName)
  {
    var person = await serviceProvider.GetRequiredService<IDataPortal<FakePerson>>().CreateAsync();
    person.FirstName = firstName;
    person.LastName = "Person";
    person.HomeTelephone = "555-1234";
    person.EmailAddresses.Add(serviceProvider.GetRequiredService<IChildDataPortal<FakePersonEmailAddress>>().CreateChild());
    person.EmailAddresses[0].EmailAddress = $"{firstName}@example.com";
    return person;
  }

  private static ApplicationContext ContextOf(object obj) => ((IUseApplicationContext)obj).ApplicationContext;

  private static IServiceScope CreateCircuitScope(IServiceProvider provider)
  {
    var scope = provider.CreateScope();
    scope.ServiceProvider.GetRequiredService<ActiveCircuitState>().CircuitExists = true;
    return scope;
  }

  private static ServiceProvider BuildProvider(bool autoCloneOnUpdate = true)
  {
    var httpContext = new DefaultHttpContext();
    httpContext.Request.Headers.Cookie = $"cslaSessionId={Guid.NewGuid()}";
    httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

    var services = new ServiceCollection();
    services.AddSingleton<IHttpContextAccessor>(new FixedHttpContextAccessor(httpContext));
    services.AddScoped<AuthenticationStateProvider, AnonymousAuthenticationStateProvider>();
    services.AddCsla(o => o
      .AddAspNetCore()
      .AddServerSideBlazor(o => o.UseInMemoryApplicationContextManager = false)
      .DataPortal(dpo => dpo.AddClientSideDataPortal(c => c.AutoCloneOnUpdate = autoCloneOnUpdate)));
    return services.BuildServiceProvider();
  }

  private sealed class FixedHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
  {
    public HttpContext HttpContext { get; set; } = httpContext;
  }

  private sealed class AnonymousAuthenticationStateProvider : AuthenticationStateProvider
  {
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
      => Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
  }
}

/// <summary>
/// Records whether its insert ran with the data portal's
/// ApplicationContext.
/// </summary>
[Serializable]
public class ScopeRecorder : BusinessBase<ScopeRecorder>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<bool> InsertUsedDataPortalScopeProperty = RegisterProperty<bool>(nameof(InsertUsedDataPortalScope));
  public bool InsertUsedDataPortalScope
  {
    get => GetProperty(InsertUsedDataPortalScopeProperty);
    private set => LoadProperty(InsertUsedDataPortalScopeProperty, value);
  }

  [Create]
  private void Create()
  {
    BusinessRules.CheckRules();
  }

  [Insert]
  private void Insert([Inject] ApplicationContext applicationContext)
  {
    InsertUsedDataPortalScope = ReferenceEquals(applicationContext, ApplicationContext);
  }
}
