﻿#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="ContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse </summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Data.Linq;

using Csla.Properties;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse 
  /// LINQ data context objects within the context
  /// of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of database 
  /// LINQ data context objects object to use.
  /// </typeparam>
  /// <remarks>
  /// This type stores the LINQ data context object 
  /// in <see cref="Csla.ApplicationContext.LocalContext" />
  /// and uses reference counting through
  /// <see cref="IDisposable" /> to keep the data context object
  /// open for reuse by child objects, and to automatically
  /// dispose the object when the last consumer
  /// has called Dispose."
  /// </remarks>
  [Obsolete("Use dependency injection", true)]
  public class ContextManager<C> : IDisposable, Core.IUseApplicationContext 
    where C : DataContext
  {
    private static object _lock = new object();
    private string _connectionString;
    private string _label;

    private ApplicationContext _applicationContext;
    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value; }

    /// <summary>
    /// Gets the ContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    public ContextManager<C> GetManager(string database)
    {
      return GetManager(database, true);
    }

    /// <summary>
    /// Gets the ContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    /// <param name="label">Label for this context.</param>
    public ContextManager<C> GetManager(string database, string label)
    {
      return GetManager(database, true, label);
    }

    /// <summary>
    /// Gets the ContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// The database name or connection string.
    /// </param>
    /// <param name="isDatabaseName">
    /// True to indicate that the connection string
    /// should be retrieved from the config file. If
    /// False, the database parameter is directly 
    /// used as a connection string.
    /// </param>
    /// <returns>ContextManager object for the name.</returns>
    public ContextManager<C> GetManager(string database, bool isDatabaseName)
    {
      return GetManager(database, isDatabaseName, "default");
    }

    /// <summary>
    /// Gets the ContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// The database name or connection string.
    /// </param>
    /// <param name="isDatabaseName">
    /// True to indicate that the connection string
    /// should be retrieved from the config file. If
    /// False, the database parameter is directly 
    /// used as a connection string.
    /// </param>
    /// <param name="label">Label for this context.</param>
    /// <returns>ContextManager object for the name.</returns>
    public ContextManager<C> GetManager(string database, bool isDatabaseName, string label)
    {
      if (isDatabaseName)
      {
        var connection = ConfigurationManager.ConnectionStrings[database];
        if (connection == null)
          throw new System.Configuration.ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        var conn = ConfigurationManager.ConnectionStrings[database].ConnectionString;
        if (string.IsNullOrEmpty(conn))
          throw new System.Configuration.ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        database = conn;
      }

      lock (_lock)
      {
        var contextLabel = GetContextName(database, label);
        ContextManager<C> mgr = null;
        if (_applicationContext.LocalContext.Contains(contextLabel))
        {
          mgr = (ContextManager<C>)(_applicationContext.LocalContext[contextLabel]);

        }
        else
        {
          mgr = new ContextManager<C>(database, label);
          _applicationContext.LocalContext[contextLabel] = mgr;
        }
        mgr.AddRef();
        return mgr;
      }
    }

    private ContextManager(string connectionString, string label)
    {
      _label = label;
      _connectionString = connectionString;

      DataContext = (C)(_applicationContext.CreateInstanceDI(typeof(C), connectionString));

    }

    private static string GetContextName(string connectionString, string label)
    {
      return "__ctx:" + label + "-" + connectionString;
    }

    /// <summary>
    /// Gets the LINQ data context object.
    /// </summary>
    public C DataContext { get; }

    #region  Reference counting

    /// <summary>
    /// Gets the current reference count for this
    /// object.
    /// </summary>
    public int RefCount { get; private set; }

    private void AddRef()
    {
      RefCount += 1;
    }

    private void DeRef()
    {

      lock (_lock)
      {
        RefCount -= 1;
        if (RefCount == 0)
        {
          DataContext.Dispose();
          _applicationContext.LocalContext.Remove(GetContextName(_connectionString, _label));
        }
      }

    }

#endregion

#region  IDisposable

    /// <summary>
    /// Dispose object, dereferencing or
    /// disposing the context it is
    /// managing.
    /// </summary>
    public void Dispose()
    {
      DeRef();
    }

#endregion

  }
}
#endif