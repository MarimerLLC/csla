//-----------------------------------------------------------------------
// <copyright file="NotNullDataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the data portal check of not-null properties</summary>
//-----------------------------------------------------------------------

#nullable enable

using System.Transactions;
using Csla.Core;
using Csla.Server;
using Csla.Testing;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.NotNull
{
  [TestClass]
  public class NotNullDataPortalTests
  {
    private static CslaTestHost _testHost = default!;

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

    [TestInitialize]
    public void TestInitialize()
    {
      NotNullPortalLog.Clear();
    }

    private static T New<T>() => _testHost.ApplicationContext.CreateInstanceDI<T>();

    private static InvalidOperationException FindViolation(Exception ex)
    {
      for (Exception? e = ex; e != null; e = e.InnerException)
      {
        if (e is InvalidOperationException ioe && ioe.Message.Contains("not-null properties"))
          return ioe;
      }
      // thrown directly, not through an assertion, so it also fails at once inside an AssertionScope
      throw new AssertionFailedException($"No not-null violation in {ex}");
    }

    private static bool IsEntryViolation(InvalidOperationException ex) => ex.Message.Contains("was not started");

    // Create

    [TestMethod]
    public async Task DataPortal_CreateLeavesNotNullUnloaded_ThrowsOnExitNamingEveryProperty()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var ex = (await FluentActions.Awaiting(() => portal.CreateAsync(NotNullPortalMode.LoadNothing)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        IsEntryViolation(violation).Should().BeFalse();
        violation.Message.Should().Contain(typeof(NotNullPortalRoot).FullName!);
        violation.Message.Should().Contain(nameof(NotNullPortalRoot.Name));
        violation.Message.Should().Contain(nameof(NotNullPortalRoot.Data));
        violation.Message.Should().NotContain(nameof(NotNullPortalRoot.Code), "a defaulted property is satisfied");
        NotNullPortalLog.Contains("Create").Should().BeTrue("the operation ran");
      }
    }

    [TestMethod]
    public async Task DataPortal_CreateLoadsAll_UnloadedDefaultedPropertyIsSilent()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var obj = await portal.CreateAsync(NotNullPortalMode.LoadAll);

      obj.Code.Should().Be("unknown");
    }

    [TestMethod]
    public async Task DataPortal_CreateOfCommand_IsNotChecked()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalCommand>();

      var cmd = await portal.CreateAsync();

      cmd.Should().NotBeNull();
    }

    // Fetch

    [TestMethod]
    public async Task DataPortal_FetchLeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var ex = (await FluentActions.Awaiting(() => portal.FetchAsync(NotNullPortalMode.LoadNameOnly)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        violation.Message.Should().Contain(nameof(NotNullPortalRoot.Data));
        violation.Message.Should().NotContain(nameof(NotNullPortalRoot.Name) + ",");
        NotNullPortalLog.Contains("Fetch").Should().BeTrue();
      }
    }

    [TestMethod]
    public async Task DataPortal_FetchLoadsAll_Succeeds()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var obj = await portal.FetchAsync(NotNullPortalMode.LoadAll);

      obj.Name.Should().Be("name");
    }

    [TestMethod]
    public async Task DataPortal_ReadOnlyFetchLeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalInfo>();

      var ex = (await FluentActions.Awaiting(() => portal.FetchAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;

      FindViolation(ex).Message.Should().Contain(nameof(NotNullPortalInfo.Name));
      (await portal.FetchAsync(true)).Name.Should().Be("x");
    }

    [TestMethod]
    public async Task DataPortal_ObjectFactoryFetchLeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullFactoryRoot>();

      var ex = (await FluentActions.Awaiting(() => portal.FetchAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;

      FindViolation(ex).Message.Should().Contain(nameof(NotNullFactoryRoot.Name));
      (await portal.FetchAsync(true)).Name.Should().Be("factory");
    }

    // Execute

    [TestMethod]
    public async Task DataPortal_ExecuteCommandObject_LeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalCommand>();
      var cmd = await portal.CreateAsync();

      var ex = (await FluentActions.Awaiting(() => portal.ExecuteAsync(cmd)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        IsEntryViolation(violation).Should().BeFalse("a command is not checked on entry");
        NotNullPortalLog.Contains("Execute").Should().BeTrue();
      }
    }

    [TestMethod]
    public async Task DataPortal_ExecuteByCriteria_LeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalCommand>();

      var ex = (await FluentActions.Awaiting(() => portal.ExecuteAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;

      IsEntryViolation(FindViolation(ex)).Should().BeFalse();
      (await portal.ExecuteAsync(true)).Result.Should().Be("done");
    }

    // Update

    [TestMethod]
    public async Task DataPortal_UpdateWithNullNotNullField_ThrowsOnEntryWithoutRunning()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();
      obj.PlantNull(NotNullPortalRoot.NameProperty);

      var ex = (await FluentActions.Awaiting(() => portal.UpdateAsync(obj)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        IsEntryViolation(violation).Should().BeTrue();
        violation.Message.Should().Contain(nameof(NotNullPortalRoot.Name));
        NotNullPortalLog.Contains("Insert").Should().BeFalse("the operation did not start");
      }
    }

    [TestMethod]
    public async Task DataPortal_UpdateLeavesNullInNotNullField_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();
      obj.NullNameOnInsert = true;

      var ex = (await FluentActions.Awaiting(() => portal.UpdateAsync(obj)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        IsEntryViolation(violation).Should().BeFalse();
        NotNullPortalLog.Contains("Insert").Should().BeTrue("in Manual mode the operation has run");
      }
    }

    [TestMethod]
    public async Task DataPortal_DeleteSelf_IsCheckedOnEntryAndExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var entry = New<NotNullPortalRoot>();
      entry.LoadAll();
      entry.MarkOldForTest();
      entry.Delete();
      entry.PlantNull(NotNullPortalRoot.DataProperty);
      var entryEx = (await FluentActions.Awaiting(() => portal.UpdateAsync(entry)).Should().ThrowAsync<DataPortalException>()).Which;
      var entryViolation = FindViolation(entryEx);
      using (new AssertionScope())
      {
        IsEntryViolation(entryViolation).Should().BeTrue();
        NotNullPortalLog.Contains("DeleteSelf").Should().BeFalse();
      }

      var exit = New<NotNullPortalRoot>();
      exit.LoadAll();
      exit.MarkOldForTest();
      exit.Delete();
      exit.NullNameOnInsert = true;
      var exitEx = (await FluentActions.Awaiting(() => portal.UpdateAsync(exit)).Should().ThrowAsync<DataPortalException>()).Which;
      var exitViolation = FindViolation(exitEx);
      using (new AssertionScope())
      {
        IsEntryViolation(exitViolation).Should().BeFalse();
        NotNullPortalLog.Contains("DeleteSelf").Should().BeTrue();
      }
    }

    [TestMethod]
    public async Task DataPortal_DeleteByCriteria_IsNotChecked()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      await portal.DeleteAsync(NotNullPortalMode.LoadNothing);

      NotNullPortalLog.Contains("Delete").Should().BeTrue();
    }

    [TestMethod]
    public async Task DataPortal_TransactionScopeExitViolation_LeavesScopeWithoutComplete()
    {
      var portal = _testHost.GetDataPortal<NotNullTransactionalRoot>();
      var obj = New<NotNullTransactionalRoot>();

      await FluentActions.Awaiting(() => portal.UpdateAsync(obj)).Should().ThrowAsync<DataPortalException>();

      NotNullTransactionalRoot.LastStatus.Should().Be(TransactionStatus.Aborted);
    }

    // Child data portal

    [TestMethod]
    public async Task ChildDataPortal_CreateAndFetchLeaveNotNullUnloaded_ThrowOnExit()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();

      var createEx = (await FluentActions.Awaiting(() => portal.CreateChildAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;
      IsEntryViolation(FindViolation(createEx)).Should().BeFalse();
      var fetchEx = (await FluentActions.Awaiting(() => portal.FetchChildAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;
      IsEntryViolation(FindViolation(fetchEx)).Should().BeFalse();
      using (new AssertionScope())
      {
        NotNullPortalLog.Contains("Child.Exception").Should().BeTrue("a failed exit check is reported as an exception");
        NotNullPortalLog.Contains("Child.InvokeComplete").Should().BeFalse("a failed exit check is not reported as a completed operation");
      }

      (await portal.CreateChildAsync(true)).Name.Should().Be("child");
      (await portal.FetchChildAsync(true)).Name.Should().Be("child");
    }

    [TestMethod]
    public async Task ChildDataPortal_DirtyChildWithNull_ThrowsOnUpdateEntry()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();
      var child = await portal.CreateChildAsync(true);
      child.PlantNull(NotNullPortalChild.NameProperty);
      NotNullPortalLog.Clear();

      var ex = (await FluentActions.Awaiting(() => portal.UpdateChildAsync(child)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        IsEntryViolation(violation).Should().BeTrue();
        NotNullPortalLog.Contains("InsertChild").Should().BeFalse();
        NotNullPortalLog.Contains("Child.Invoke").Should().BeFalse("the operation did not start");
        NotNullPortalLog.Contains("Child.Exception").Should().BeFalse("no exception notification is raised when the operation did not start");
      }
    }

    [TestMethod]
    public async Task ChildDataPortal_CleanChildWithNull_IsNotChecked()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();
      var child = await portal.FetchChildAsync(true);
      child.PlantNull(NotNullPortalChild.NameProperty);
      child.MarkCleanForTest();

      await portal.UpdateChildAsync(child);

      NotNullPortalLog.Contains("UpdateChild").Should().BeFalse();
    }

    [TestMethod]
    public async Task ChildDataPortal_DeleteSelfChild_IsCheckedOnEntryAndExit()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();

      var entry = await portal.FetchChildAsync(true);
      entry.DeleteForTest();
      entry.PlantNull(NotNullPortalChild.NameProperty);
      var entryEx = (await FluentActions.Awaiting(() => portal.UpdateChildAsync(entry)).Should().ThrowAsync<DataPortalException>()).Which;
      var entryViolation = FindViolation(entryEx);
      using (new AssertionScope())
      {
        IsEntryViolation(entryViolation).Should().BeTrue();
        NotNullPortalLog.Contains("DeleteSelfChild").Should().BeFalse();
      }

      var exit = await portal.FetchChildAsync(true);
      exit.DeleteForTest();
      exit.NullNameOnSave = true;
      var exitEx = (await FluentActions.Awaiting(() => portal.UpdateChildAsync(exit)).Should().ThrowAsync<DataPortalException>()).Which;
      var exitViolation = FindViolation(exitEx);
      using (new AssertionScope())
      {
        IsEntryViolation(exitViolation).Should().BeFalse();
        NotNullPortalLog.Contains("DeleteSelfChild").Should().BeTrue();
      }
    }

    [TestMethod]
    public async Task ChildDataPortal_ExecuteChildCommand_IsCheckedOnExitOnly()
    {
      var portal = _testHost.GetChildDataPortal<NotNullChildCommand>();

      var cmd = await portal.CreateChildAsync();
      var ex = (await FluentActions.Awaiting(() => portal.UpdateChildAsync(cmd)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        IsEntryViolation(violation).Should().BeFalse();
        NotNullPortalLog.Contains("ExecuteChild").Should().BeTrue();
      }
    }

    [TestMethod]
    public void NotNullInvariant_ObjectWithoutFieldManager_ReportsNoDefaultPropertiesWithoutCreatingIt()
    {
      var obj = New<NotNullPortalRoot>();
      ((IFieldManagerAccess)obj).FieldManagerIfCreated.Should().BeNull();

      var failing = NotNullInvariant.GetUnsatisfied(obj).Select(p => p.Name).ToArray();

      using (new AssertionScope())
      {
        failing.Should().BeEquivalentTo(new[] { nameof(NotNullPortalRoot.Name), nameof(NotNullPortalRoot.Data) });
        ((IFieldManagerAccess)obj).FieldManagerIfCreated.Should().BeNull("the check does not create a field manager");
      }
    }

    [TestMethod]
    public void NotNullInvariant_List_IsSkipped()
    {
      NotNullInvariant.GetUnsatisfied(New<NotNullPortalList>()).Should().BeEmpty();
    }

    // Hierarchies and the backstop

    [TestMethod]
    public async Task DataPortal_PropertiesFromGenericAndNonGenericBase_AreChecked()
    {
      var portal = _testHost.GetDataPortal<NotNullLeaf>();

      var ex = (await FluentActions.Awaiting(() => portal.CreateAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;

      var violation = FindViolation(ex);
      using (new AssertionScope())
      {
        violation.Message.Should().Contain(nameof(NotNullLeaf.LayerName));
        violation.Message.Should().Contain(nameof(NotNullLeaf.MiddleName));
        violation.Message.Should().Contain(nameof(NotNullLeaf.LeafName));
      }
      (await portal.CreateAsync(true)).LeafName.Should().Be("leaf");
    }

    [TestMethod]
    public void Backstop_NotSerializedNotNullWithoutDefault_ThrowsOnEveryInstantiation()
    {
      for (var i = 0; i < 2; i++)
      {
        var obj = New<NotNullNotSerializedRoot>();
        var ex = FluentActions.Invoking(() => _ = ((IUseFieldManager)obj).FieldManager).Should().ThrowExactly<InvalidOperationException>().Which;
        ex.Message.Should().Contain(nameof(NotNullNotSerializedRoot.Transient));
      }
    }

    [TestMethod]
    public void Backstop_NotSerializedNotNullWithDefault_IsAllowed()
    {
      var obj = New<NotNullNotSerializedDefaultRoot>();

      obj.Transient.Should().Be("d");
    }
  }

  public enum NotNullPortalMode
  {
    LoadNothing,
    LoadNameOnly,
    LoadAll
  }

  internal static class NotNullPortalLog
  {
    private static readonly List<string> _entries = [];
    public static void Add(string entry) { lock (_entries) _entries.Add(entry); }
    public static bool Contains(string entry) { lock (_entries) return _entries.Contains(entry); }
    public static void Clear() { lock (_entries) _entries.Clear(); }
  }

  [Serializable]
  public class NotNullPortalRoot : BusinessBase<NotNullPortalRoot>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    public static readonly NotNullPropertyInfo<byte[]> DataProperty = RegisterNotNullProperty<byte[]>(nameof(Data));
    public byte[] Data => GetProperty(DataProperty);

    public static readonly NotNullPropertyInfo<string> CodeProperty = RegisterNotNullProperty<string>(nameof(Code), null, "unknown");
    public string Code => GetProperty(CodeProperty);

    public static readonly PropertyInfo<bool> NullNameOnInsertProperty = RegisterProperty<bool>(nameof(NullNameOnInsert));
    public bool NullNameOnInsert
    {
      get => GetProperty(NullNameOnInsertProperty);
      set => SetProperty(NullNameOnInsertProperty, value);
    }

    public void LoadAll()
    {
      LoadProperty(NameProperty, "name");
      LoadProperty(DataProperty, [1]);
    }

    public void PlantNull(IPropertyInfo property) => FieldManager.LoadFieldData(property, null);
    public void MarkOldForTest() => MarkOld();

    private void Load(NotNullPortalMode mode)
    {
      if (mode >= NotNullPortalMode.LoadNameOnly)
        LoadProperty(NameProperty, "name");
      if (mode >= NotNullPortalMode.LoadAll)
        LoadProperty(DataProperty, [1]);
    }

    [Create]
    private void Create(NotNullPortalMode mode)
    {
      NotNullPortalLog.Add("Create");
      Load(mode);
    }

    [Fetch]
    private void Fetch(NotNullPortalMode mode)
    {
      NotNullPortalLog.Add("Fetch");
      Load(mode);
    }

    [Insert]
    private void Insert()
    {
      NotNullPortalLog.Add("Insert");
      if (NullNameOnInsert)
        PlantNull(NameProperty);
    }

    [Update]
    private void Update()
    {
      NotNullPortalLog.Add("Update");
    }

    [DeleteSelf]
    private void DeleteSelf()
    {
      NotNullPortalLog.Add("DeleteSelf");
      if (NullNameOnInsert)
        PlantNull(NameProperty);
    }

    [Delete]
    private void Delete(NotNullPortalMode mode)
    {
      NotNullPortalLog.Add("Delete");
    }
  }

  [Serializable]
  public class NotNullPortalList : BusinessListBase<NotNullPortalList, NotNullPortalChild>
  {
  }

  [Serializable]
  public class NotNullTransactionalRoot : BusinessBase<NotNullTransactionalRoot>
  {
    public static TransactionStatus? LastStatus;

    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name), null, "x");
    public string Name => GetProperty(NameProperty);

    [Insert]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Insert()
    {
      LastStatus = null;
      Transaction.Current!.TransactionCompleted += (s, e) => LastStatus = e.Transaction!.TransactionInformation.Status;
      FieldManager.LoadFieldData(NameProperty, null);
    }
  }

  [Serializable]
  public class NotNullPortalInfo : ReadOnlyBase<NotNullPortalInfo>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    [Fetch]
    private void Fetch(bool load)
    {
      if (load)
        LoadProperty(NameProperty, "x");
    }
  }

  [Serializable]
  [ObjectFactory(typeof(NotNullRootFactory))]
  public class NotNullFactoryRoot : BusinessBase<NotNullFactoryRoot>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);
  }

  public class NotNullRootFactory : ObjectFactory
  {
    public NotNullRootFactory(ApplicationContext applicationContext) : base(applicationContext)
    { }

    [Fetch]
    public NotNullFactoryRoot Fetch(bool load)
    {
      var obj = ApplicationContext.CreateInstanceDI<NotNullFactoryRoot>();
      if (load)
        LoadProperty(obj, NotNullFactoryRoot.NameProperty, "factory");
      MarkOld(obj);
      return obj;
    }
  }

  [Serializable]
  public class NotNullPortalCommand : CommandBase<NotNullPortalCommand>
  {
    public static readonly NotNullPropertyInfo<string> ResultProperty = RegisterNotNullProperty<string>(nameof(Result));
    public string Result => ReadProperty(ResultProperty);

    [Create]
    private void Create()
    {
    }

    [Execute]
    private void Execute()
    {
      NotNullPortalLog.Add("Execute");
    }

    [Execute]
    private void Execute(bool load)
    {
      if (load)
        LoadProperty(ResultProperty, "done");
    }
  }

  [Serializable]
  public class NotNullChildCommand : CommandBase<NotNullChildCommand>
  {
    public static readonly NotNullPropertyInfo<string> ResultProperty = RegisterNotNullProperty<string>(nameof(Result));
    public string Result => ReadProperty(ResultProperty);

    [CreateChild]
    private void CreateChild()
    {
    }

    [ExecuteChild]
    private void ExecuteChild()
    {
      NotNullPortalLog.Add("ExecuteChild");
    }
  }

  [Serializable]
  public class NotNullPortalChild : BusinessBase<NotNullPortalChild>
  {
    public static readonly NotNullPropertyInfo<string> NameProperty = RegisterNotNullProperty<string>(nameof(Name));
    public string Name => GetProperty(NameProperty);

    [NonSerialized]
    private bool _nullNameOnSave;
    public bool NullNameOnSave
    {
      get => _nullNameOnSave;
      set => _nullNameOnSave = value;
    }

    public void PlantNull(IPropertyInfo property) => FieldManager.LoadFieldData(property, null);
    public void MarkCleanForTest() => MarkClean();
    public void DeleteForTest() => MarkDeleted();

    protected override void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      NotNullPortalLog.Add("Child.Invoke");
      base.Child_OnDataPortalInvoke(e);
    }

    protected override void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      NotNullPortalLog.Add("Child.InvokeComplete");
      base.Child_OnDataPortalInvokeComplete(e);
    }

    protected override void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      NotNullPortalLog.Add("Child.Exception");
      base.Child_OnDataPortalException(e, ex);
    }

    [CreateChild]
    private void CreateChild(bool load)
    {
      if (load)
        LoadProperty(NameProperty, "child");
    }

    [FetchChild]
    private void FetchChild(bool load)
    {
      if (load)
        LoadProperty(NameProperty, "child");
    }

    [InsertChild]
    private void InsertChild()
    {
      NotNullPortalLog.Add("InsertChild");
    }

    [UpdateChild]
    private void UpdateChild()
    {
      NotNullPortalLog.Add("UpdateChild");
    }

    [DeleteSelfChild]
    private void DeleteSelfChild()
    {
      NotNullPortalLog.Add("DeleteSelfChild");
      if (NullNameOnSave)
        PlantNull(NameProperty);
    }
  }

  [Serializable]
  public abstract class NotNullGenericLayer<T> : BusinessBase<T>
    where T : NotNullGenericLayer<T>
  {
    public static readonly NotNullPropertyInfo<string> LayerNameProperty = RegisterNotNullProperty<string>(nameof(LayerName));
    public string LayerName => GetProperty(LayerNameProperty);
  }

  [Serializable]
  public abstract class NotNullNonGenericLayer : NotNullGenericLayer<NotNullLeaf>
  {
    public static readonly PropertyInfo<string> MiddleNameProperty =
      RegisterProperty(typeof(NotNullNonGenericLayer), new NotNullPropertyInfo<string>(nameof(MiddleName), null, typeof(NotNullNonGenericLayer)));
    public string MiddleName => GetProperty(MiddleNameProperty)!;
  }

  [Serializable]
  public class NotNullLeaf : NotNullNonGenericLayer
  {
    public static readonly NotNullPropertyInfo<string> LeafNameProperty = RegisterNotNullProperty<string>(nameof(LeafName));
    public string LeafName => GetProperty(LeafNameProperty);

    [Create]
    private void Create(bool load)
    {
      if (load)
      {
        LoadProperty(LayerNameProperty, "layer");
        LoadProperty(MiddleNameProperty, "middle");
        LoadProperty(LeafNameProperty, "leaf");
      }
    }
  }

  [Serializable]
  public class NotNullNotSerializedRoot : BusinessBase<NotNullNotSerializedRoot>
  {
    public static readonly NotNullPropertyInfo<string> TransientProperty = RegisterNotNullProperty<string>(nameof(Transient));
    [DoNotSerialize]
    public string Transient => GetProperty(TransientProperty);
  }

  [Serializable]
  public class NotNullNotSerializedDefaultRoot : BusinessBase<NotNullNotSerializedDefaultRoot>
  {
    public static readonly NotNullPropertyInfo<string> TransientProperty = RegisterNotNullProperty<string>(nameof(Transient), null, "d");
    [DoNotSerialize]
    public string Transient => GetProperty(TransientProperty);
  }
}
