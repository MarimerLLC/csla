using Csla.Rules;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.Test
{
  [TestClass]
  public class ViewModelAsyncAuthorizationRuleTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public async Task RefreshAsync_WithAsyncAuthorizationRules_SetsObjectLevelPermissions()
    {
      var vm = new ViewModel<AsyncAuthRoot>(_testHost.ApplicationContext);

      await vm.RefreshAsync(() => Task.FromResult<AsyncAuthRoot>(null));

      Assert.IsNull(vm.Exception);
      Assert.IsFalse(vm.CanCreateObject);
      Assert.IsTrue(vm.CanGetObject);
      Assert.IsTrue(vm.CanEditObject);
      Assert.IsFalse(vm.CanDeleteObject);
    }

    [TestMethod]
    public void CanCreateObject_WithAsyncAuthorizationRules_DoesNotThrow()
    {
      var vm = new ViewModel<AsyncAuthRoot>(_testHost.ApplicationContext);

      // the rule has not completed yet, so the default value is returned
      Assert.IsFalse(vm.CanCreateObject);
    }

    [TestMethod]
    public async Task SaveAsync_WithPendingAsyncIsSavableRule_WaitsAndSaves()
    {
      var model = await _testHost.GetDataPortal<AsyncSavableRoot>().CreateAsync();
      var vm = new ViewModel<AsyncSavableRoot>(_testHost.ApplicationContext)
      {
        ManageObjectLifetime = false,
        Model = model,
      };
      var saved = false;
      vm.Saved += () => saved = true;

      await vm.SaveAsync();

      Assert.IsNull(vm.Exception, vm.Exception?.ToString());
      Assert.IsTrue(saved);
      Assert.IsFalse(vm.Model.IsNew);
    }
  }

  [Serializable]
  public class AsyncSavableRoot : BusinessBase<AsyncSavableRoot>
  {
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      // IsSavable checks CreateObject for a new object
      BusinessRules.AddRule(typeof(AsyncSavableRoot), new AsyncAuthRoot.DelayedAuthRule(AuthorizationActions.CreateObject, true));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [Insert]
    private void Insert()
    { }
  }

  [Serializable]
  public class AsyncAuthRoot : BusinessBase<AsyncAuthRoot>
  {
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(AsyncAuthRoot), new DelayedAuthRule(AuthorizationActions.CreateObject, false));
      BusinessRules.AddRule(typeof(AsyncAuthRoot), new DelayedAuthRule(AuthorizationActions.GetObject, true));
      BusinessRules.AddRule(typeof(AsyncAuthRoot), new DelayedAuthRule(AuthorizationActions.DeleteObject, false));
    }

    internal class DelayedAuthRule : AuthorizationRuleAsync
    {
      private readonly bool _hasPermission;

      public DelayedAuthRule(AuthorizationActions action, bool hasPermission)
        : base(action)
      {
        _hasPermission = hasPermission;
      }

      protected override async Task ExecuteAsync(IAuthorizationContext context, CancellationToken ct)
      {
        await Task.Delay(10, ct);
        context.HasPermission = _hasPermission;
      }
    }
  }
}
