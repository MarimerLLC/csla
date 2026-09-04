//-----------------------------------------------------------------------
// <copyright file="AssemblyLoadContextUnloadTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Verifies static per-type caches are flushed when a collectible AssemblyLoadContext unloads</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Reflection;
using Csla.Rules;
using Csla.Server;
using Csla.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Runtime
{
  /// <summary>
  /// Business type used only by <see cref="AssemblyLoadContextUnloadTests"/> so no other
  /// test populates the framework caches for it from the default load context.
  /// </summary>
  [Serializable]
  public class UnloadTestRoot : BusinessBase<UnloadTestRoot>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(DataProperty));
    }

    [Create]
    private void Create()
    {
      Data = "created";
    }
  }

  [TestClass]
  public class AssemblyLoadContextUnloadTests
  {
    private const string ContextName = "CslaUnloadTestContext";

    /// <summary>
    /// Every static per-type cache in the framework that registers an
    /// <see cref="AssemblyLoadContext.Unloading"/> handler through
    /// <see cref="Csla.Runtime.AssemblyLoadContextManager"/>.
    /// </summary>
    private static readonly (Type Owner, string Field)[] _caches =
    [
      (typeof(PropertyInfoManager), "_propertyInfoCache"),
      (typeof(FieldDataManager), "_consolidatedLists"),
      (typeof(UndoableHandler), "_undoableFieldCache"),
      (typeof(MethodCaller), "_methodCache"),
      (typeof(MethodCaller), "_memberCache"),
      (typeof(ServiceProviderMethodCaller), "_methodCache"),
      (typeof(DataPortalMethodCache), "_cache"),
      (typeof(DataPortalTarget), "_methodNameList"),
      (typeof(ObjectFactoryAttribute), "_cache"),
      (typeof(BusinessRuleManager), "_perTypeRules"),
      (typeof(AuthorizationRuleManager), "_perTypeRules"),
    ];

    private sealed class CollectibleContext : AssemblyLoadContext
    {
      public CollectibleContext() : base(ContextName, isCollectible: true) { }

      // Return null so every dependency (including Csla itself) resolves from the
      // default context; only the test assembly is loaded into this context.
      protected override Assembly Load(AssemblyName assemblyName) => null;
    }

    [TestMethod]
    public void UnloadingCollectibleContext_FlushesCachesAndDoesNotThrow()
    {
      using var testHost = CslaTestHost.Create();
      var context = new CollectibleContext();
      Type rootType;

      using (context.EnterContextualReflection())
      {
        var assembly = context.LoadFromAssemblyPath(typeof(UnloadTestRoot).Assembly.Location);
        rootType = assembly.GetType(typeof(UnloadTestRoot).FullName);
        Assert.IsNotNull(rootType);
        Assert.AreNotSame(typeof(UnloadTestRoot), rootType, "type should come from the collectible context");
        Assert.IsTrue(rootType.Assembly.IsCollectible);

        // Run a data portal operation plus an undo cycle on the collectible type so
        // the static per-type caches get entries tagged with this context.
        var portalType = typeof(IDataPortal<>).MakeGenericType(rootType);
        var portal = (IDataPortal)testHost.Services.GetRequiredService(portalType);
        var obj = portal.Create();
        Assert.IsInstanceOfType(obj, rootType);

        var undoable = (ISupportUndo)obj;
        undoable.BeginEdit();
        undoable.CancelEdit();
      }

      var populated = _caches.Where(c => CountEntriesForContext(c.Owner, c.Field) > 0).ToList();
      Assert.IsTrue(populated.Count >= 5,
        $"expected most caches to hold entries for the collectible context, found {populated.Count}: {Describe(populated)}");

      // Unloading raises AssemblyLoadContext.Unloading synchronously, which is what
      // triggers each cache's flush handler. Any exception in a handler surfaces here.
      context.Unload();

      var stale = _caches.Where(c => CountEntriesForContext(c.Owner, c.Field) > 0).ToList();
      Assert.AreEqual(0, stale.Count, $"caches still holding entries for the unloaded context: {Describe(stale)}");
    }

    private static string Describe(IEnumerable<(Type Owner, string Field)> caches)
      => string.Join(", ", caches.Select(c => $"{c.Owner.Name}.{c.Field}"));

    /// <summary>
    /// Counts entries in a cache whose value is a (contextName, item) tuple tagged
    /// with the collectible context's name.
    /// </summary>
    private static int CountEntriesForContext(Type owner, string fieldName)
    {
      var field = owner.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
      Assert.IsNotNull(field, $"{owner.Name}.{fieldName} not found");

      var value = field.GetValue(null);
      if (value is null)
        return 0;

      // The rule managers wrap their dictionary in Lazy<T>
      if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(Lazy<>))
      {
        var isValueCreated = (bool)value.GetType().GetProperty(nameof(Lazy<object>.IsValueCreated)).GetValue(value);
        if (!isValueCreated)
          return 0;
        value = value.GetType().GetProperty(nameof(Lazy<object>.Value)).GetValue(value);
      }

      var dictionary = (IDictionary)value;
      var count = 0;
      foreach (DictionaryEntry entry in dictionary)
      {
        if (entry.Value is ITuple tuple && tuple.Length == 2 && Equals(tuple[0], ContextName))
          count++;
      }
      return count;
    }
  }
}
