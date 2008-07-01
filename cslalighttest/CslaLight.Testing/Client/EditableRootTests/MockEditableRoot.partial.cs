using System;
using Csla.Serialization;
using System.Diagnostics;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableRootTests
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  partial class MockEditableRoot
  {
    public MockEditableRoot() { }
    public MockEditableRoot(Guid id, bool isnew)
    {
      SetProperty<Guid>(IdProperty, id);
      if (!isnew)
        MarkOld();
    }

    #region  Factory Methods

    public static void CreateNew(EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      DataPortal.BeginCreate<MockEditableRoot>(completed);
    }

    public static void Fetch(Guid id, EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      DataPortal.BeginFetch<MockEditableRoot>(
        new SingleCriteria<MockEditableRoot, Guid>(id),
        completed);
    }

    public static void Delete(Guid id) { Delete(id, null); }
    public static void Delete(Guid id, EventHandler<DataPortalResult<MockEditableRoot>> completed)
    {
      DataPortal.BeginDelete<MockEditableRoot>(
        new SingleCriteria<MockEditableRoot, Guid>(id),
        completed);
    }

    #endregion

    // For local data portal tests
    #region Data Access

    public void DataPortal_Create(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      //ValidationRules.CheckRules();

      LoadProperty<string>(DataPortalMethodProperty, "create");

      handler(this, null);
    }

    public void DataPortal_Fetch(
      LocalProxy<MockEditableRoot>.CompletedHandler handler,
      SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      if (criteria.Value != MockEditableRootId)
        throw new Exception();

      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      LoadProperty<string>(DataPortalMethodProperty, "fetch");

      handler(this, null);
    }

    public void DataPortal_Insert(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "insert");
      handler(this, null);
    }

    public void DataPortal_Update(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "update");
      handler(this, null);
    }

    public void DataPortal_DeleteSelf(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      DataPortal_Delete(handler, new SingleCriteria<MockEditableRoot, Guid>(this.Id));
      handler(this, null);
    }

    public void DataPortal_Delete(
      LocalProxy<MockEditableRoot>.CompletedHandler handler,
      SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      LoadProperty<string>(DataPortalMethodProperty, "delete");
      handler(this, null);
    }

    #endregion
  }
}
