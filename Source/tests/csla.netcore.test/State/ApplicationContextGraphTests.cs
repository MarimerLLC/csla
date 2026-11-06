//-----------------------------------------------------------------------
// <copyright file="ApplicationContextGraphTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests attaching session object graphs to an ApplicationContext</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Csla.State;
using Csla.Test.BasicModern;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.State
{
  [TestClass]
  public class ApplicationContextGraphTests
  {
    private static ServiceProvider BuildProvider()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      return services.BuildServiceProvider();
    }

    private static Root CreateGraph(IServiceProvider serviceProvider)
    {
      var root = serviceProvider.GetRequiredService<IDataPortal<Root>>().Create();
      var child = serviceProvider.GetRequiredService<IChildDataPortal<Child>>().CreateChild(1, "child");
      root.Children.Add(child);
      return root;
    }

    private static ApplicationContext ContextOf(object obj) => ((IUseApplicationContext)obj).ApplicationContext;

    [TestMethod]
    public void SessionValuesAreAttached()
    {
      using var provider = BuildProvider();
      using var oldScope = provider.CreateScope();
      using var newScope = provider.CreateScope();
      var newContext = newScope.ServiceProvider.GetRequiredService<ApplicationContext>();

      var direct = CreateGraph(oldScope.ServiceProvider);
      var inContextDictionary = CreateGraph(oldScope.ServiceProvider);
      var inList = CreateGraph(oldScope.ServiceProvider);
      var localContext = new ContextDictionary { ["root"] = inContextDictionary };
      var session = new Session
      {
        ["direct"] = direct,
        ["local"] = localContext,
        ["list"] = new MobileList<Root> { inList },
        ["text"] = "not a graph",
        ["bytes"] = new byte[] { 1, 2, 3 },
        ["nothing"] = null
      };
      Assert.AreNotSame(newContext, ContextOf(direct));

      ApplicationContextGraph.SetApplicationContext(session, newContext);

      foreach (var root in new[] { direct, inContextDictionary, inList })
      {
        Assert.AreSame(newContext, ContextOf(root), "root");
        Assert.AreSame(newContext, ContextOf(root.Children), "child list");
        Assert.AreSame(newContext, ContextOf(root.Children[0]), "child");
      }
    }

    [TestMethod]
    public void ObjectsInManagedContainersAreAttached()
    {
      using var provider = BuildProvider();
      using var oldScope = provider.CreateScope();
      using var newScope = provider.CreateScope();
      var newContext = newScope.ServiceProvider.GetRequiredService<ApplicationContext>();

      var holder = oldScope.ServiceProvider.GetRequiredService<IDataPortal<ContainerHolder>>().Create();
      var inDictionary = CreateGraph(oldScope.ServiceProvider);
      var inNestedList = CreateGraph(oldScope.ServiceProvider);
      holder.Dictionary.Add("root", inDictionary);
      holder.NestedList.Add(new MobileList<Root> { inNestedList });
      var session = new Session { ["holder"] = holder };

      ApplicationContextGraph.SetApplicationContext(session, newContext);

      Assert.AreSame(newContext, ContextOf(holder), "holder");
      foreach (var root in new[] { inDictionary, inNestedList })
      {
        Assert.AreSame(newContext, ContextOf(root), "root");
        Assert.AreSame(newContext, ContextOf(root.Children[0]), "child");
      }
    }

    [TestMethod]
    public void ContainerInObjectTypedPropertyIsAttached()
    {
      using var provider = BuildProvider();
      using var oldScope = provider.CreateScope();
      using var newScope = provider.CreateScope();
      var newContext = newScope.ServiceProvider.GetRequiredService<ApplicationContext>();

      var holder = oldScope.ServiceProvider.GetRequiredService<IDataPortal<ContainerHolder>>().Create();
      var root = CreateGraph(oldScope.ServiceProvider);
      holder.SetUntyped(new MobileDictionary<string, Root> { ["root"] = root });
      var session = new Session { ["holder"] = holder };

      ApplicationContextGraph.SetApplicationContext(session, newContext);

      Assert.AreSame(newContext, ContextOf(root), "root");
      Assert.AreSame(newContext, ContextOf(root.Children[0]), "child");
    }

    [TestMethod]
    public void CriteriaWithPlaceholderContextIsAttached()
    {
      using var provider = BuildProvider();
      using var scope = provider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
      var criteria = new SessionCriteria();
      var session = new Session { ["criteria"] = criteria };

      ApplicationContextGraph.SetApplicationContext(session, context);

      Assert.AreSame(context, ContextOf(criteria));
    }

    [TestMethod]
    public void DictionaryKeysAreAttached()
    {
      using var provider = BuildProvider();
      using var oldScope = provider.CreateScope();
      using var newScope = provider.CreateScope();
      var newContext = newScope.ServiceProvider.GetRequiredService<ApplicationContext>();

      var key = CreateGraph(oldScope.ServiceProvider);
      var session = new Session { ["keyed"] = new MobileDictionary<Root, string> { [key] = "value" } };

      ApplicationContextGraph.SetApplicationContext(session, newContext);

      Assert.AreSame(newContext, ContextOf(key), "root");
      Assert.AreSame(newContext, ContextOf(key.Children[0]), "child");
    }

    [TestMethod]
    public void CyclicContainersTerminate()
    {
      using var provider = BuildProvider();
      using var oldScope = provider.CreateScope();
      using var newScope = provider.CreateScope();
      var newContext = newScope.ServiceProvider.GetRequiredService<ApplicationContext>();

      var root = CreateGraph(oldScope.ServiceProvider);
      var outer = new ContextDictionary();
      var inner = new ContextDictionary { ["outer"] = outer, ["root"] = root };
      outer["inner"] = inner;
      var session = new Session { ["outer"] = outer };
      session["self"] = session;

      ApplicationContextGraph.SetApplicationContext(session, newContext);

      Assert.AreSame(newContext, ContextOf(root));
    }

    [TestMethod]
    public void AttachedGraphIsLeftAlone()
    {
      using var provider = BuildProvider();
      using var scope = provider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
      var root = CreateGraph(scope.ServiceProvider);
      var session = new Session { ["root"] = root };

      ApplicationContextGraph.SetApplicationContext(session, context);

      Assert.AreSame(context, ContextOf(root));
      Assert.AreSame(context, ContextOf(root.Children[0]));
    }
  }

  [Serializable]
  public class ContainerHolder : BusinessBase<ContainerHolder>
  {
    public static readonly PropertyInfo<MobileDictionary<string, Root>> DictionaryProperty = RegisterProperty<MobileDictionary<string, Root>>(nameof(Dictionary));
    public MobileDictionary<string, Root> Dictionary
    {
      get => GetProperty(DictionaryProperty);
      private set => LoadProperty(DictionaryProperty, value);
    }

    public static readonly PropertyInfo<MobileList<MobileList<Root>>> NestedListProperty = RegisterProperty<MobileList<MobileList<Root>>>(nameof(NestedList));
    public MobileList<MobileList<Root>> NestedList
    {
      get => GetProperty(NestedListProperty);
      private set => LoadProperty(NestedListProperty, value);
    }

    public static readonly PropertyInfo<object> UntypedProperty = RegisterProperty<object>(nameof(Untyped));
    public object Untyped => GetProperty(UntypedProperty);

    public void SetUntyped(object value) => LoadProperty(UntypedProperty, value);

    [Create]
    private void Create()
    {
      Dictionary = [];
      NestedList = [];
    }
  }

#pragma warning disable CS0618 // CriteriaBase is obsolete, but still supported
  [Serializable]
  public class SessionCriteria : CriteriaBase<SessionCriteria>;
#pragma warning restore CS0618
}
