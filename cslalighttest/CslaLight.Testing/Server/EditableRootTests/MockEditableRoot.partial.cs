using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.EditableRootTests
{
  partial class MockEditableRoot
  {
    private MockEditableRoot()
    { /* require use of factory methods */ }

    #region  Data Access

    [RunLocal()]
    protected override void DataPortal_Create()
    {
      LoadProperty<Guid>(IdProperty, Guid.NewGuid());
      ValidationRules.CheckRules();
    }

    private void DataPortal_Fetch(SingleCriteria<MockEditableRoot, Guid> criteria)
    {
        LoadProperty<Guid>(IdProperty, MockEditableRootId);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new SingleCriteria<MockEditableRoot, Guid>(ReadProperty<Guid>(IdProperty)));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();
    }

    #endregion
  }
}
