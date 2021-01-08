//-----------------------------------------------------------------------
// <copyright file="DashboardFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates a dashboard instance based on configuration</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;

namespace Csla.Server.Dashboard
{
  /// <summary>
  /// Creates a dashboard instance based on configuration.
  /// </summary>
  public static class DashboardFactory
  {
    private static IDashboard _dashboard;

    /// <summary>
    /// Gets a dashboard instance.
    /// </summary>
    /// <returns></returns>
    public static IDashboard GetDashboard()
    {
      IDashboard result = null;
      if (_dashboard == null)
      {
        var typeName = ConfigurationManager.AppSettings["CslaDashboardType"];
        if (!string.IsNullOrWhiteSpace(typeName))
        {
          if (typeName == "Dashboard")
            result = new Dashboard();
          else if (typeName == "NullDashboard")
            result = new NullDashboard();
          else
            result = (IDashboard)Reflection.MethodCaller.CreateInstance(Type.GetType(typeName));
        }
        else
        {
          result = new NullDashboard();
        }
        _dashboard = result;
      }
      else
      {
        result = _dashboard;
      }
      return result;
    }

    /// <summary>
    /// Resets the dashboard.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void Reset()
    {
      _dashboard?.Dispose();
      _dashboard = null;
      DataPortal.Dashboard = GetDashboard();
    }
  }
}
