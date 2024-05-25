﻿//-----------------------------------------------------------------------
// <copyright file="IDataPortalTarget.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  internal interface IDataPortalTarget
  {
    void MarkAsChild();
    void MarkNew();
    void MarkOld();
    void CheckRules();
    Task CheckRulesAsync();
    Task WaitForIdle(TimeSpan timeout);
    Task WaitForIdle(CancellationToken ct);
    void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e);
    void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e);
    void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex);
    void Child_OnDataPortalInvoke(DataPortalEventArgs e);
    void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e);
    void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex);
  }
}