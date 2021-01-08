#if !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="ObjectContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse </summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Data.Objects;
using Csla.Properties;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse 
  /// Entity Framework object context objects 
  /// within the context
  /// of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of database 
  /// object context object to use.
  /// </typeparam>
  /// <remarks>
  /// This type stores the object context object 
  /// in <see cref="Csla.ApplicationContext.LocalContext" />
  /// and uses reference counting through
  /// <see cref="IDisposable" /> to keep the data context object
  /// open for reuse by child objects, and to automatically
  /// dispose the object when the last consumer
  /// has called Dispose."
  /// </remarks>
  public class ObjectContextManager<C> : IDisposable where C : ObjectContext
  {
    private static object _lock = new object();
    private C _context;
    private string _connectionString;
    private string _label;

    /// <summary>
    /// Gets the ObjectContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    public static ObjectContextManager<C> GetManager(string database)
    {
      return GetManager(database, true);
    }

    /// <summary>
    /// Gets the ObjectContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    /// <param name="label">Label for this context.</param>
    public static ObjectContextManager<C> GetManager(string database, string label)
    {
      return GetManager(database, true, label);
    }

    /// <summary>
    /// Gets the ObjectContextManager object for the 
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
    public static ObjectContextManager<C> GetManager(string database, bool isDatabaseName)
    {
      return GetManager(database, isDatabaseName, "default");
    }

    /// <summary>
    /// Gets the ObjectContextManager object for the 
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
    public static ObjectContextManager<C> GetManager(string database, bool isDatabaseName, string label)
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
        ObjectContextManager<C> mgr = null;
        if (ApplicationContext.LocalContext.Contains(contextLabel))
        {
          mgr = (ObjectContextManager<C>)(ApplicationContext.LocalContext[contextLabel]);

        }
        else
        {
          mgr = new ObjectContextManager<C>(database, label);
          ApplicationContext.LocalContext[contextLabel] = mgr;
        }
        mgr.AddRef();
        return mgr;
      }
    }

    private ObjectContextManager(string connectionString, string label)
    {
      _label = label;
      _connectionString = connectionString;

      _context = (C)(Reflection.MethodCaller.CreateInstance(typeof(C), connectionString));
      _context.Connection.Open();
    }

    private static string GetContextName(string connectionString, string label)
    {
      return "__octx:" + label + "-" + connectionString;
    }


    /// <summary>
    /// Gets the EF object context object.
    /// </summary>
    public C ObjectContext
    {
      get
      {
        return _context;
      }
    }

#region  Reference counting

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

      lock (_lock)
      {
        _refCount -= 1;
        if (_refCount == 0)
        {
          _context.Connection.Close();
          _context.Dispose();
          ApplicationContext.LocalContext.Remove(GetContextName(_connectionString, _label));
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