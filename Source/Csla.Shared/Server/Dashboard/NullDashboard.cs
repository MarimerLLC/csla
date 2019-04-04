//-----------------------------------------------------------------------
// <copyright file="NullDashboard.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Data portal dashboard that does nothing</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Csla.Server.Dashboard
{
  /// <summary>
  /// Data portal server dashboard that records 
  /// no data and consumes no meaningful overhead.
  /// </summary>
  public class NullDashboard : IDashboard
  {
    DateTimeOffset IDashboard.FirstCall => throw new NotImplementedException();

    DateTimeOffset IDashboard.LastCall => throw new NotImplementedException();

    long IDashboard.TotalCalls => throw new NotImplementedException();

    long IDashboard.FailedCalls => throw new NotImplementedException();

    long IDashboard.CompletedCalls => throw new NotImplementedException();

    void IDashboard.InitializeCall(InterceptArgs e)
    {
    }

    void IDashboard.CompleteCall(InterceptArgs e)
    {
    }

    List<Activity> IDashboard.GetRecentActivity()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Dispose resources used by this object.
    /// </summary>
    public void Dispose()
    {
    }
  }
}
