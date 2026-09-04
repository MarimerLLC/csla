//-----------------------------------------------------------------------
// <copyright file="HostingTestObjects.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business objects used by the test host tests</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;
using Csla.Core;

namespace Csla.Testing.Tests.Hosting
{
  /// <summary>
  /// Service used to prove that a fake registered through ConfigureServices is
  /// resolved by the object graph the data portal creates.
  /// </summary>
  public interface INameSource
  {
    string GetName(int id);
  }

  /// <summary>
  /// Test implementation of <see cref="INameSource"/>.
  /// </summary>
  public class FakeNameSource : INameSource
  {
    public string GetName(int id) => $"Name {id}";
  }

  /// <summary>
  /// Root object with a data portal fetch, so a test can prove the host stands
  /// up a working data portal rather than merely resolving one.
  /// </summary>
  [Serializable]
  public class HostedRoot : BusinessBase<HostedRoot>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      private set => LoadProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      private set => LoadProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<string> UserNameProperty = RegisterProperty<string>(nameof(UserName));
    public string UserName
    {
      get => GetProperty(UserNameProperty);
      private set => LoadProperty(UserNameProperty, value);
    }

    [Fetch]
    private void Fetch(int id, [Inject] INameSource nameSource)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = nameSource.GetName(id);
        UserName = ApplicationContext.User.Identity?.Name ?? string.Empty;
      }
    }
  }

  /// <summary>
  /// Child object, so a test can prove the child data portal resolves.
  /// </summary>
  [Serializable]
  public class HostedChild : BusinessBase<HostedChild>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      private set => LoadProperty(IdProperty, value);
    }

    [FetchChild]
    private void FetchChild(int id)
    {
      using (BypassPropertyChecks)
        Id = id;
    }
  }

  /// <summary>
  /// Scoped service used to observe that disposing the host disposes the
  /// service provider it owns.
  /// </summary>
  public class DisposalSentinel : IDisposable
  {
    public bool IsDisposed { get; private set; }

    public void Dispose() => IsDisposed = true;
  }

  /// <summary>
  /// Handler a test registers itself. CSLA registers this service with a plain
  /// AddScoped rather than TryAdd, so it pins the registration ordering contract.
  /// </summary>
  public class FakeUnhandledAsyncRuleExceptionHandler : Csla.Rules.IUnhandledAsyncRuleExceptionHandler
  {
    public bool CanHandle(Exception exception, Csla.Rules.IBusinessRuleBase executingRule) => true;

    public ValueTask Handle(Exception exception, Csla.Rules.IBusinessRuleBase executingRule, Csla.Rules.IRuleContext ruleContext)
      => default;
  }

  /// <summary>
  /// Context manager a test registers itself, to prove the TryAdd contract.
  /// </summary>
  public class CustomContextManager : ApplicationContextManagerAsyncLocal
  {
    public bool WasUsed { get; private set; }

    public override IPrincipal GetUser()
    {
      WasUsed = true;
      return base.GetUser();
    }
  }
}
