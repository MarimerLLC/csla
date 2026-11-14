//-----------------------------------------------------------------------
// <copyright file="CommandWaitForIdleTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests that the data portal waits for a command's busy children</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Csla.Test.ValidationRules;
using Csla.Testing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.CommandBase
{
  /// <summary>
  /// A command has no busy state of its own, so the data portal waits
  /// for the objects it holds in managed properties. Otherwise a remote
  /// data portal serializes the command while a child's async rule is
  /// still running, and the rule's result never reaches the caller (#4991).
  /// </summary>
  [TestClass]
  public class CommandWaitForIdleTests
  {
    [TestMethod]
    public async Task Create_WaitsForChildAsyncRule()
    {
      await RunAndAssert(portal => portal.CreateAsync());
    }

    [TestMethod]
    public async Task Execute_WaitsForChildAsyncRule()
    {
      await RunAndAssert(async portal => await portal.ExecuteAsync(await portal.CreateAsync("empty")));
    }

    private static async Task RunAndAssert(Func<IDataPortal<BusyChildCommand>, Task<BusyChildCommand>> operation)
    {
      using var host = CslaTestHost.Create(t => t.ConfigureServices(s => s.AddSingleton<RuleGates>()));
      var gates = host.Services.GetRequiredService<RuleGates>();

      var running = Task.Run(() => operation(host.GetDataPortal<BusyChildCommand>()));

      // the child's rule has started and is waiting at its gate
      var deadline = DateTime.UtcNow.AddSeconds(10);
      while (!gates.IsWaitingAt(BusyCommandChild.NameRule))
      {
        if (running.IsCompleted || DateTime.UtcNow > deadline)
          Assert.Fail("The child's async rule never started.");
        await Task.Delay(10);
      }

      // the data portal doesn't return the command while the child is busy
      await Task.Delay(250);
      running.IsCompleted.Should().BeFalse();

      gates.Release(BusyCommandChild.NameRule);
      var command = await running;

      command.Child.IsBusy.Should().BeFalse();
      command.Child.IsValid.Should().BeFalse();

      // what a remote data portal would have sent back to the caller
      var copy = command.Clone();
      copy.Child.IsValid.Should().BeFalse();
      copy.Child.BrokenRulesCollection.Should().ContainSingle();
    }
  }

  public class BusyChildCommand : CommandBase<BusyChildCommand>
  {
    public static readonly PropertyInfo<BusyCommandChild> ChildProperty = RegisterProperty<BusyCommandChild>(nameof(Child));
    public BusyCommandChild Child
    {
      get => ReadProperty(ChildProperty);
      private set => LoadProperty(ChildProperty, value);
    }

    [Create]
    private void Create([Inject] IChildDataPortal<BusyCommandChild> childPortal)
    {
      Child = childPortal.CreateChild();
    }

    [Create]
    private void Create(string empty)
    {
      _ = empty;
    }

    [Execute]
    private void Execute([Inject] IChildDataPortal<BusyCommandChild> childPortal)
    {
      Child = childPortal.CreateChild();
    }
  }

  public class BusyCommandChild : BusinessBase<BusyCommandChild>
  {
    public const string NameRule = "CommandChildName";

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    internal RuleGates Gates => ApplicationContext.GetRequiredService<RuleGates>();

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new GatedInvalidRule(NameProperty));
    }

    [CreateChild]
    private void CreateChild()
    {
      // starts the async rule and returns while it is still running
      BusinessRules.CheckRules();
    }

    private class GatedInvalidRule : BusinessRuleAsync
    {
      public GatedInvalidRule(Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        ProvideTargetWhenAsync = true;
      }

      protected override async Task ExecuteAsync(IRuleContext context)
      {
        await ((BusyCommandChild)context.Target).Gates.WaitAt(NameRule);
        context.AddErrorResult("Name is not valid");
      }
    }
  }
}
