//-----------------------------------------------------------------------
// <copyright file="MockEditableRoot.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using Csla.DataPortalClient;
#endif

namespace Csla.Testing.Business.EditableRootTests
{
  partial class MockEditableRoot
  {
    #region  Data Access

#if SILVERLIGHT
    public override void DataPortal_Create(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      //ValidationRules.CheckRules();

      LoadProperty<string>(DataPortalMethodProperty, "create");

      handler(this, null);
    }
#else
    [RunLocal()]
    protected override void DataPortal_Create()
    {
      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      BusinessRules.CheckRules();

      LoadProperty<string>(DataPortalMethodProperty, "create");
    }
#endif

#if SILVERLIGHT
    public void DataPortal_Fetch(
      SingleCriteria<MockEditableRoot, Guid> criteria,
      LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      if (criteria.Value != MockEditableRootId)
        throw new Exception();

      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      LoadProperty<string>(DataPortalMethodProperty, "fetch");

      handler(this, null);
    }
#else
    private void DataPortal_Fetch(SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      if (criteria.Value != MockEditableRootId)
        throw new Exception();

      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      LoadProperty<string>(DataPortalMethodProperty, "fetch");
    }
#endif

#if SILVERLIGHT
    public override void DataPortal_Insert(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "insert");
      handler(this, null);
    }
#else
    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "insert");
    }
#endif

#if SILVERLIGHT
    public override void DataPortal_Update(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "update");
      handler(this, null);
    }
#else
    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "update");
    }
#endif

#if SILVERLIGHT
    public override void DataPortal_DeleteSelf(LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      DataPortal_Delete(new SingleCriteria<MockEditableRoot, Guid>(this.Id), handler);
      handler(this, null);
    }
#else
    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new SingleCriteria<MockEditableRoot, Guid>(ReadProperty<Guid>(IdProperty)));
    }
#endif

#if SILVERLIGHT
    public void DataPortal_Delete(
      SingleCriteria<MockEditableRoot, Guid> criteria,
      LocalProxy<MockEditableRoot>.CompletedHandler handler)
    {
      LoadProperty<string>(DataPortalMethodProperty, "delete");
      handler(this, null);
    }
#else
    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "delete");
    }
#endif
    #endregion
  }
}