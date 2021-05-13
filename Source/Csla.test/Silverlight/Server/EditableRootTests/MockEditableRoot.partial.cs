//-----------------------------------------------------------------------
// <copyright file="MockEditableRoot.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.EditableRootTests
{
  partial class MockEditableRoot
  {
    [RunLocal()]
    [Create]
		protected void DataPortal_Create()
    {
      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      BusinessRules.CheckRules();

      LoadProperty<string>(DataPortalMethodProperty, "create");
    }

    private void DataPortal_Fetch(Guid criteria)
    {
      if (criteria != MockEditableRootId)
        throw new Exception();

      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      LoadProperty<string>(DataPortalMethodProperty, "fetch");
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Insert]
    protected void DataPortal_Insert()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "insert");
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Update]
		protected void DataPortal_Update()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "update");
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty<Guid>(IdProperty));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Delete]
    private void DataPortal_Delete(Guid criteria)
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "delete");
    }
  }
}