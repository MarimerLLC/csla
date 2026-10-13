//-----------------------------------------------------------------------
// <copyright file="TransactionScopeFactoryTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for ITransactionScopeFactory</summary>
//-----------------------------------------------------------------------

using System.Transactions;
using Csla.Configuration;
using Csla.Server;
using Csla.Testing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class TransactionScopeFactoryTests
  {
    [TestMethod]
    public void TransactionalAttribute_WithoutExplicitValues_LeavesThemUnspecified()
    {
      var attribute = new TransactionalAttribute();

      attribute.TransactionType.Should().Be(TransactionalTypes.TransactionScope);
      attribute.TransactionIsolationLevel.Should().Be(TransactionIsolationLevel.Unspecified);
      attribute.TimeoutInSeconds.Should().BeNull();
      attribute.AsyncFlowOption.Should().BeNull();
    }

    [TestMethod]
    public void TransactionalAttribute_WithExplicitValues_KeepsThem()
    {
      var attribute = new TransactionalAttribute(TransactionalTypes.TransactionScope, TransactionIsolationLevel.ReadCommitted, 100, TransactionScopeAsyncFlowOption.Enabled);

      attribute.TransactionIsolationLevel.Should().Be(TransactionIsolationLevel.ReadCommitted);
      attribute.TimeoutInSeconds.Should().Be(100);
      attribute.AsyncFlowOption.Should().Be(TransactionScopeAsyncFlowOption.Enabled);
    }

    [TestMethod]
    public void DefaultFactory_UsesDataOptionsDefaults_WhenAttributeIsUnspecified()
    {
      var options = new CslaOptions(new ServiceCollection());
      options.DataOptions.DefaultTransactionIsolationLevel = TransactionIsolationLevel.ReadCommitted;
      var factory = new DefaultTransactionScopeFactory(options);

      using var scope = factory.CreateTransactionScope(CreateContext(new TransactionalAttribute()));

      Transaction.Current.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
    }

    [TestMethod]
    public void DefaultFactory_UsesAttributeValues_WhenSpecified()
    {
      var options = new CslaOptions(new ServiceCollection());
      options.DataOptions.DefaultTransactionIsolationLevel = TransactionIsolationLevel.ReadCommitted;
      var factory = new DefaultTransactionScopeFactory(options);

      using var scope = factory.CreateTransactionScope(CreateContext(new TransactionalAttribute(TransactionalTypes.TransactionScope, TransactionIsolationLevel.RepeatableRead)));

      Transaction.Current.IsolationLevel.Should().Be(IsolationLevel.RepeatableRead);
    }

    [TestMethod]
    public void DataPortal_UsesDefaultIsolationLevel_OfEachHost()
    {
      using var readCommittedHost = CslaTestHost.Create(t => t.ConfigureCsla(o => o
        .Data(d => d.DefaultTransactionIsolationLevel = TransactionIsolationLevel.ReadCommitted)));
      using var repeatableReadHost = CslaTestHost.Create(t => t.ConfigureCsla(o => o
        .Data(d => d.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead)));

      var readCommitted = readCommittedHost.GetDataPortal<TransactionScopeFactoryRoot>().Fetch();
      var repeatableRead = repeatableReadHost.GetDataPortal<TransactionScopeFactoryRoot>().Fetch();

      readCommitted.IsolationLevel.Should().Be(nameof(IsolationLevel.ReadCommitted));
      repeatableRead.IsolationLevel.Should().Be(nameof(IsolationLevel.RepeatableRead));
    }

    [TestMethod]
    public void DataPortal_UsesAttributeIsolationLevel_OverDefault()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(o => o
        .Data(d => d.DefaultTransactionIsolationLevel = TransactionIsolationLevel.ReadCommitted)));

      var root = testHost.GetDataPortal<TransactionScopeFactoryRoot>().Fetch(42);

      root.IsolationLevel.Should().Be(nameof(IsolationLevel.Serializable));
    }

    [TestMethod]
    public void DataPortal_UsesRegisteredTransactionScopeFactory()
    {
      var recorder = new TransactionScopeFactoryRecorder();
      using var testHost = CslaTestHost.Create(t => t
        .ConfigureServices(s => s.AddSingleton(recorder))
        .ConfigureCsla(o => o.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterTransactionScopeFactory<RecordingTransactionScopeFactory>()))));

      var root = testHost.GetDataPortal<TransactionScopeFactoryRoot>().Fetch();

      root.IsolationLevel.Should().Be(nameof(IsolationLevel.ReadUncommitted));
      recorder.Contexts.Should().ContainSingle();
      var context = recorder.Contexts[0];
      context.ObjectType.Should().Be(typeof(TransactionScopeFactoryRoot));
      context.Operation.Should().Be(DataPortalOperations.Fetch);
      context.MethodName.Should().Be("Fetch");
      context.Method.GetParameters().Should().BeEmpty();
      context.TransactionalAttribute.TransactionType.Should().Be(TransactionalTypes.TransactionScope);
      context.BusinessObject.Should().BeNull();
      context.Criteria.Should().BeEmpty();
    }

    [TestMethod]
    public void DataPortal_PassesCriteriaToTransactionScopeFactory()
    {
      var recorder = new TransactionScopeFactoryRecorder();
      using var testHost = CreateRecordingHost(recorder);

      testHost.GetDataPortal<TransactionScopeFactoryRoot>().Fetch(42);

      recorder.Contexts.Should().ContainSingle();
      recorder.Contexts[0].Operation.Should().Be(DataPortalOperations.Fetch);
      recorder.Contexts[0].Criteria.Should().Equal(42);
      recorder.Contexts[0].BusinessObject.Should().BeNull();
    }

    [TestMethod]
    public void DataPortal_PassesMultipleCriteriaToTransactionScopeFactory()
    {
      var recorder = new TransactionScopeFactoryRecorder();
      using var testHost = CreateRecordingHost(recorder);

      testHost.GetDataPortal<TransactionScopeFactoryRoot>().Delete(42, "batch");

      recorder.Contexts.Should().ContainSingle();
      recorder.Contexts[0].Operation.Should().Be(DataPortalOperations.Delete);
      recorder.Contexts[0].Criteria.Should().Equal(42, "batch");
      recorder.Contexts[0].BusinessObject.Should().BeNull();
    }

    [TestMethod]
    public void DataPortal_PassesUpdateOperationToTransactionScopeFactory()
    {
      var recorder = new TransactionScopeFactoryRecorder();
      using var testHost = CslaTestHost.Create(t => t
        .ConfigureServices(s => s.AddSingleton(recorder))
        .ConfigureCsla(o => o.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterTransactionScopeFactory<RecordingTransactionScopeFactory>()))));
      var dataPortal = testHost.GetDataPortal<TransactionScopeFactoryRoot>();

      var root = dataPortal.Create();
      root.Name = "new";
      root = dataPortal.Update(root);

      root.IsolationLevel.Should().Be(nameof(IsolationLevel.ReadUncommitted));
      recorder.Contexts.Should().ContainSingle();
      recorder.Contexts[0].Operation.Should().Be(DataPortalOperations.Update);
      recorder.Contexts[0].MethodName.Should().Be("Insert");
      recorder.Contexts[0].BusinessObject.Should().BeOfType<TransactionScopeFactoryRoot>()
        .Which.Name.Should().Be("new");
      recorder.Contexts[0].Criteria.Should().BeEmpty();
    }

    private static CslaTestHost CreateRecordingHost(TransactionScopeFactoryRecorder recorder)
      => CslaTestHost.Create(t => t
        .ConfigureServices(s => s.AddSingleton(recorder))
        .ConfigureCsla(o => o.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterTransactionScopeFactory<RecordingTransactionScopeFactory>()))));

    private static TransactionScopeFactoryContext CreateContext(TransactionalAttribute attribute)
      => new(typeof(TransactionScopeFactoryRoot), DataPortalOperations.Fetch, typeof(TransactionScopeFactoryTests).GetMethod(nameof(CreateContext), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static), attribute);
  }

  public class TransactionScopeFactoryRecorder
  {
    public List<TransactionScopeFactoryContext> Contexts { get; } = [];
  }

  public class RecordingTransactionScopeFactory(TransactionScopeFactoryRecorder recorder) : ITransactionScopeFactory
  {
    public TransactionScope CreateTransactionScope(TransactionScopeFactoryContext context)
    {
      recorder.Contexts.Add(context);
      return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });
    }
  }

  [Serializable]
  public class TransactionScopeFactoryRoot : BusinessBase<TransactionScopeFactoryRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<string> IsolationLevelProperty = RegisterProperty<string>(nameof(IsolationLevel));
    public string IsolationLevel
    {
      get => GetProperty(IsolationLevelProperty);
      private set => LoadProperty(IsolationLevelProperty, value);
    }

    [RunLocal]
    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Fetch]
    private void Fetch()
    {
      IsolationLevel = Transaction.Current?.IsolationLevel.ToString();
    }

    [Transactional(TransactionalTypes.TransactionScope, TransactionIsolationLevel.Serializable)]
    [Fetch]
    private void Fetch(int id)
    {
      IsolationLevel = Transaction.Current?.IsolationLevel.ToString();
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Delete]
    private void Delete(int id, string reason)
    {
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Insert]
    private void Insert()
    {
      IsolationLevel = Transaction.Current?.IsolationLevel.ToString();
    }
  }
}
