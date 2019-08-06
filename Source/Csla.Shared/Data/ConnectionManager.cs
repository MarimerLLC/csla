#if !NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse open</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Data;
using System.Data.Common;
using Csla.Properties;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse open
  /// database connections within the context
  /// of a single data portal operation.
  /// </summary>
  /// <remarks>
  /// This type stores the open database connection
  /// in <see cref="Csla.ApplicationContext.LocalContext" />
  /// and uses reference counting through
  /// <see cref="IDisposable" /> to keep the connection
  /// open for reuse by child objects, and to automatically
  /// dispose the connection when the last consumer
  /// has called Dispose."
  /// </remarks>
  public class ConnectionManager : IDisposable
  {
    private static object _lock = new object();
    private IDbConnection _connection;
    private string _connectionString;
    private string _label;

    /// <summary>
    /// Gets the ConnectionManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    public static ConnectionManager GetManager(string database)
    {
      return GetManager(database, true);
    }

    /// <summary>
    /// Gets the ConnectionManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    /// <param name="label">Label for this connection.</param>
    public static ConnectionManager GetManager(string database, string label)
    {
      return GetManager(database, true, label);
    }

    /// <summary>
    /// Gets the ConnectionManager object for the 
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
    /// <returns>ConnectionManager object for the name.</returns>
    public static ConnectionManager GetManager(string database, bool isDatabaseName)
    {
      return GetManager(database, isDatabaseName, "default");
    }

    /// <summary>
    /// Gets the ConnectionManager object for the 
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
    /// <param name="label">Label for this connection.</param>
    /// <returns>ConnectionManager object for the name.</returns>
    public static ConnectionManager GetManager(string database, bool isDatabaseName, string label)
    {
      if (isDatabaseName)
      {
        var connection = ConfigurationManager.ConnectionStrings[database];
        if (connection == null)
          throw new ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));

        var conn = ConfigurationManager.ConnectionStrings[database].ConnectionString;
        if (string.IsNullOrEmpty(conn))
          throw new ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        database = conn;
      }

      ConnectionManager mgr = null;
      var ctxName = GetContextName(database, label);
      lock (_lock)
      {
        var cached = ApplicationContext.LocalContext.GetValueOrNull(ctxName);
        if (cached != null)
        {
          mgr = (ConnectionManager)cached;
          mgr.AddRef();
        }
      }

      if (mgr == null)
      {
        mgr = new ConnectionManager(database, label);
        lock (_lock)
        {
          ApplicationContext.LocalContext[ctxName] = mgr;
          mgr.AddRef();
        }
      }
      return mgr;
    }

    private ConnectionManager(string connectionString, string label)
    {
      _label = label;
      _connectionString = connectionString;

#if NETSTANDARD2_0
      _connection = new System.Data.SqlClient.SqlConnection(connectionString);
      _connection.Open();
#else
      string provider = ConfigurationManager.AppSettings["CslaDbProvider"];
      if (string.IsNullOrEmpty(provider))
        provider = "System.Data.SqlClient";

      DbProviderFactory factory = DbProviderFactories.GetFactory(provider);

      // open connection
      _connection = factory.CreateConnection();
      _connection.ConnectionString = connectionString;
      _connection.Open();
#endif
    }

    private static string GetContextName(string connectionString, string label)
    {
      return "__db:" + label + "-" + connectionString;
    }

    /// <summary>
    /// Dispose object, dereferencing or
    /// disposing the connection it is
    /// managing.
    /// </summary>
    public IDbConnection Connection
    {
      get
      {
        return _connection;
      }
    }

    private int _refCount;

    /// <summary>
    /// Gets the current reference count for this
    /// object.
    /// </summary>
    public int RefCount
    {
      get { return _refCount; }
    }

    private void AddRef()
    {
      _refCount += 1;
    }

    private void DeRef()
    {
      _refCount -= 1;
      if (_refCount == 0)
      {
        _connection.Close();
        _connection.Dispose();
        lock (_lock)
          ApplicationContext.LocalContext.Remove(GetContextName(_connectionString, _label));
      }
    }

    /// <summary>
    /// Dispose object, dereferencing or
    /// disposing the connection it is
    /// managing.
    /// </summary>
    public void Dispose()
    {
      DeRef();
    }
  }
}
#endif