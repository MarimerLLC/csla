#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="ObjectContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse </summary>
//-----------------------------------------------------------------------
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
  public class ObjectContextManager<C> : IDisposable, Core.IUseApplicationContext
    where C : ObjectContext
  {
    private static readonly object _lock = new object();
    private string _connectionString;
    private string _label;

    private ApplicationContext _applicationContext;
    
    /// <inheritdoc />
    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext)); }

    /// <summary>
    /// Gets the ObjectContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="database"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public ObjectContextManager<C> GetManager(string database)
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
    /// <exception cref="ArgumentNullException"><paramref name="label"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="database"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public ObjectContextManager<C> GetManager(string database, string label)
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
    /// <exception cref="ArgumentException"><paramref name="database"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public ObjectContextManager<C> GetManager(string database, bool isDatabaseName)
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
    /// <exception cref="ArgumentNullException"><paramref name="label"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="database"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public ObjectContextManager<C> GetManager(string database, bool isDatabaseName, string label)
    {
      if (string.IsNullOrWhiteSpace(database))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(database)), nameof(database));
      if (label is null)
        throw new ArgumentNullException(nameof(label));

      if (isDatabaseName)
      {
        var connection = ConfigurationManager.ConnectionStrings[database];
        if (connection == null)
          throw new System.Configuration.ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        var conn = ConfigurationManager.ConnectionStrings[database].ConnectionString;
        if (string.IsNullOrEmpty(conn))
          throw new System.Configuration.ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        database = conn!;
      }

      lock (_lock)
      {
        var contextLabel = GetContextName(database, label);
        ObjectContextManager<C> mgr;
        if (_applicationContext.LocalContext.Contains(contextLabel))
        {
          mgr = (ObjectContextManager<C>)(_applicationContext.LocalContext[contextLabel]);

        }
        else
        {
          mgr = new ObjectContextManager<C>(database, label);
          _applicationContext.LocalContext[contextLabel] = mgr;
        }
        mgr.AddRef();
        return mgr;
      }
    }

    private ObjectContextManager(string connectionString, string label)
    {
      _label = label;
      _connectionString = connectionString;

      ObjectContext = (C)_applicationContext.CreateInstanceDI(typeof(C), connectionString);
      ObjectContext.Connection.Open();
    }

    private static string GetContextName(string connectionString, string label)
    {
      return "__octx:" + label + "-" + connectionString;
    }


    /// <summary>
    /// Gets the EF object context object.
    /// </summary>
    public C ObjectContext { get; }

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
          ObjectContext.Connection.Close();
          ObjectContext.Dispose();
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