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
      Assert.Fail($"No not-null violation in {ex}");
      return null!;
    }

    private static bool IsEntryViolation(InvalidOperationException ex) => ex.Message.Contains("was not started");

    // Create

    [TestMethod]
    public async Task DataPortal_CreateLeavesNotNullUnloaded_ThrowsOnExitNamingEveryProperty()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.CreateAsync(NotNullPortalMode.LoadNothing));

      var violation = FindViolation(ex);
      Assert.IsFalse(IsEntryViolation(violation));
      StringAssert.Contains(violation.Message, typeof(NotNullPortalRoot).FullName);
      StringAssert.Contains(violation.Message, nameof(NotNullPortalRoot.Name));
      StringAssert.Contains(violation.Message, nameof(NotNullPortalRoot.Data));
      Assert.IsFalse(violation.Message.Contains(nameof(NotNullPortalRoot.Code)), "A defaulted property is satisfied");
      Assert.IsTrue(NotNullPortalLog.Contains("Create"), "The operation ran");
    }

    [TestMethod]
    public async Task DataPortal_CreateLoadsAll_UnloadedDefaultedPropertyIsSilent()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var obj = await portal.CreateAsync(NotNullPortalMode.LoadAll);

      Assert.AreEqual("unknown", obj.Code);
    }

    [TestMethod]
    public async Task DataPortal_CreateOfCommand_IsNotChecked()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalCommand>();

      var cmd = await portal.CreateAsync();

      Assert.IsNotNull(cmd);
    }

    // Fetch

    [TestMethod]
    public async Task DataPortal_FetchLeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.FetchAsync(NotNullPortalMode.LoadNameOnly));

      var violation = FindViolation(ex);
      StringAssert.Contains(violation.Message, nameof(NotNullPortalRoot.Data));
      Assert.IsFalse(violation.Message.Contains(nameof(NotNullPortalRoot.Name) + ","));
      Assert.IsTrue(NotNullPortalLog.Contains("Fetch"));
    }

    [TestMethod]
    public async Task DataPortal_FetchLoadsAll_Succeeds()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      var obj = await portal.FetchAsync(NotNullPortalMode.LoadAll);

      Assert.AreEqual("name", obj.Name);
    }

    [TestMethod]
    public async Task DataPortal_ReadOnlyFetchLeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalInfo>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.FetchAsync(false));

      StringAssert.Contains(FindViolation(ex).Message, nameof(NotNullPortalInfo.Name));
      Assert.AreEqual("x", (await portal.FetchAsync(true)).Name);
    }

    [TestMethod]
    public async Task DataPortal_ObjectFactoryFetchLeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullFactoryRoot>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.FetchAsync(false));

      StringAssert.Contains(FindViolation(ex).Message, nameof(NotNullFactoryRoot.Name));
      Assert.AreEqual("factory", (await portal.FetchAsync(true)).Name);
    }

    // Execute

    [TestMethod]
    public async Task DataPortal_ExecuteCommandObject_LeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalCommand>();
      var cmd = await portal.CreateAsync();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.ExecuteAsync(cmd));

      var violation = FindViolation(ex);
      Assert.IsFalse(IsEntryViolation(violation), "A command is not checked on entry");
      Assert.IsTrue(NotNullPortalLog.Contains("Execute"));
    }

    [TestMethod]
    public async Task DataPortal_ExecuteByCriteria_LeavesNotNullUnloaded_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalCommand>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.ExecuteAsync(false));

      Assert.IsFalse(IsEntryViolation(FindViolation(ex)));
      Assert.AreEqual("done", (await portal.ExecuteAsync(true)).Result);
    }

    // Update

    [TestMethod]
    public async Task DataPortal_UpdateWithNullNotNullField_ThrowsOnEntryWithoutRunning()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();
      obj.PlantNull(NotNullPortalRoot.NameProperty);

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateAsync(obj));

      var violation = FindViolation(ex);
      Assert.IsTrue(IsEntryViolation(violation));
      StringAssert.Contains(violation.Message, nameof(NotNullPortalRoot.Name));
      Assert.IsFalse(NotNullPortalLog.Contains("Insert"), "The operation did not start");
    }

    [TestMethod]
    public async Task DataPortal_UpdateLeavesNullInNotNullField_ThrowsOnExit()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();
      obj.NullNameOnInsert = true;

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateAsync(obj));

      Assert.IsFalse(IsEntryViolation(FindViolation(ex)));
      Assert.IsTrue(NotNullPortalLog.Contains("Insert"), "In Manual mode the operation has run");
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
      var entryEx = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateAsync(entry));
      Assert.IsTrue(IsEntryViolation(FindViolation(entryEx)));
      Assert.IsFalse(NotNullPortalLog.Contains("DeleteSelf"));

      var exit = New<NotNullPortalRoot>();
      exit.LoadAll();
      exit.MarkOldForTest();
      exit.Delete();
      exit.NullNameOnInsert = true;
      var exitEx = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateAsync(exit));
      Assert.IsFalse(IsEntryViolation(FindViolation(exitEx)));
      Assert.IsTrue(NotNullPortalLog.Contains("DeleteSelf"));
    }

    [TestMethod]
    public async Task DataPortal_DeleteByCriteria_IsNotChecked()
    {
      var portal = _testHost.GetDataPortal<NotNullPortalRoot>();

      await portal.DeleteAsync(NotNullPortalMode.LoadNothing);

      Assert.IsTrue(NotNullPortalLog.Contains("Delete"));
    }

    [TestMethod]
    public async Task DataPortal_TransactionScopeExitViolation_LeavesScopeWithoutComplete()
    {
      var portal = _testHost.GetDataPortal<NotNullTransactionalRoot>();
      var obj = New<NotNullTransactionalRoot>();

      await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateAsync(obj));

      Assert.AreEqual(TransactionStatus.Aborted, NotNullTransactionalRoot.LastStatus);
    }

    // Child data portal

    [TestMethod]
    public async Task ChildDataPortal_CreateAndFetchLeaveNotNullUnloaded_ThrowOnExit()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();

      var createEx = await Assert.ThrowsAsync<DataPortalException>(() => portal.CreateChildAsync(false));
      Assert.IsFalse(IsEntryViolation(FindViolation(createEx)));
      var fetchEx = await Assert.ThrowsAsync<DataPortalException>(() => portal.FetchChildAsync(false));
      Assert.IsFalse(IsEntryViolation(FindViolation(fetchEx)));

      Assert.AreEqual("child", (await portal.CreateChildAsync(true)).Name);
      Assert.AreEqual("child", (await portal.FetchChildAsync(true)).Name);
    }

    [TestMethod]
    public async Task ChildDataPortal_DirtyChildWithNull_ThrowsOnUpdateEntry()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();
      var child = await portal.CreateChildAsync(true);
      child.PlantNull(NotNullPortalChild.NameProperty);

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateChildAsync(child));

      Assert.IsTrue(IsEntryViolation(FindViolation(ex)));
      Assert.IsFalse(NotNullPortalLog.Contains("InsertChild"));
    }

    [TestMethod]
    public async Task ChildDataPortal_CleanChildWithNull_IsNotChecked()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();
      var child = await portal.FetchChildAsync(true);
      child.PlantNull(NotNullPortalChild.NameProperty);
      child.MarkCleanForTest();

      await portal.UpdateChildAsync(child);

      Assert.IsFalse(NotNullPortalLog.Contains("UpdateChild"));
    }

    [TestMethod]
    public async Task ChildDataPortal_DeleteSelfChild_IsCheckedOnEntryAndExit()
    {
      var portal = _testHost.GetChildDataPortal<NotNullPortalChild>();

      var entry = await portal.FetchChildAsync(true);
      entry.DeleteForTest();
      entry.PlantNull(NotNullPortalChild.NameProperty);
      var entryEx = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateChildAsync(entry));
      Assert.IsTrue(IsEntryViolation(FindViolation(entryEx)));
      Assert.IsFalse(NotNullPortalLog.Contains("DeleteSelfChild"));

      var exit = await portal.FetchChildAsync(true);
      exit.DeleteForTest();
      exit.NullNameOnSave = true;
      var exitEx = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateChildAsync(exit));
      Assert.IsFalse(IsEntryViolation(FindViolation(exitEx)));
      Assert.IsTrue(NotNullPortalLog.Contains("DeleteSelfChild"));
    }

    [TestMethod]
    public async Task ChildDataPortal_ExecuteChildCommand_IsCheckedOnExitOnly()
    {
      var portal = _testHost.GetChildDataPortal<NotNullChildCommand>();

      var cmd = await portal.CreateChildAsync();
      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.UpdateChildAsync(cmd));

      Assert.IsFalse(IsEntryViolation(FindViolation(ex)));
      Assert.IsTrue(NotNullPortalLog.Contains("ExecuteChild"));
    }

    [TestMethod]
    public void NotNullInvariant_ObjectWithoutFieldManager_ReportsNoDefaultPropertiesWithoutCreatingIt()
    {
      var obj = New<NotNullPortalRoot>();
      Assert.IsNull(((IFieldManagerAccess)obj).FieldManagerIfCreated);

      var failing = NotNullInvariant.GetUnsatisfied(obj).Select(p => p.Name).ToArray();

      CollectionAssert.AreEquivalent(new[] { nameof(NotNullPortalRoot.Name), nameof(NotNullPortalRoot.Data) }, failing);
      Assert.IsNull(((IFieldManagerAccess)obj).FieldManagerIfCreated, "The check does not create a field manager");
    }

    [TestMethod]
    public void NotNullInvariant_List_IsSkipped()
    {
      Assert.AreEqual(0, NotNullInvariant.GetUnsatisfied(New<NotNullPortalList>()).Count);
    }

    // Hierarchies and the backstop

    [TestMethod]
    public async Task DataPortal_PropertiesFromGenericAndNonGenericBase_AreChecked()
    {
      var portal = _testHost.GetDataPortal<NotNullLeaf>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.CreateAsync(false));

      var violation = FindViolation(ex);
      StringAssert.Contains(violation.Message, nameof(NotNullLeaf.LayerName));
      StringAssert.Contains(violation.Message, nameof(NotNullLeaf.MiddleName));
      StringAssert.Contains(violation.Message, nameof(NotNullLeaf.LeafName));
      Assert.AreEqual("leaf", (await portal.CreateAsync(true)).LeafName);
    }

    [TestMethod]
    public void Backstop_NotSerializedNotNullWithoutDefault_ThrowsOnEveryInstantiation()
    {
      for (var i = 0; i < 2; i++)
      {
        var obj = New<NotNullNotSerializedRoot>();
        var ex = Assert.ThrowsExactly<InvalidOperationException>(() => _ = ((IUseFieldManager)obj).FieldManager);
        StringAssert.Contains(ex.Message, nameof(NotNullNotSerializedRoot.Transient));
      }
    }

    [TestMethod]
    public void Backstop_NotSerializedNotNullWithDefault_IsAllowed()
    {
      var obj = New<NotNullNotSerializedDefaultRoot>();

      Assert.AreEqual("d", obj.Transient);
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
