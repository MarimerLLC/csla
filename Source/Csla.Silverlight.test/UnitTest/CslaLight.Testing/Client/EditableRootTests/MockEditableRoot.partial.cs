//-----------------------------------------------------------------------
// <copyright file="MockEditableRoot.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableRootTests
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  partial class MockEditableRoot
  {
    // For local data portal tests
    #region Data Access

#if SILVERLIGHT

    protected override void DataPortal_Create()
    {
      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      //ValidationRules.CheckRules();

      LoadProperty<string>(DataPortalMethodProperty, "create");
    }

    public void DataPortal_Fetch(
      SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      if (criteria.Value != MockEditableRootId)
        throw new Exception();

      LoadProperty<Guid>(IdProperty, MockEditableRootId);
      LoadProperty<string>(DataPortalMethodProperty, "fetch");
    }

    protected override void DataPortal_Insert()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "insert");
    }

    protected override void DataPortal_Update()
    {
      Guid id = ReadProperty<Guid>(IdProperty);
      if (id != MockEditableRootId)
        throw new Exception();

      LoadProperty<string>(DataPortalMethodProperty, "update");
    }

    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new SingleCriteria<MockEditableRoot, Guid>(this.Id));
    }

    public void DataPortal_Delete(
      SingleCriteria<MockEditableRoot, Guid> criteria)
    {
      LoadProperty<string>(DataPortalMethodProperty, "delete");
    }

#endif

    #endregion
  }
}