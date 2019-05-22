//-----------------------------------------------------------------------
// <copyright file="SafeSqlDataReader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is a SqlDataReader based on SafeDataReader</summary>
//-----------------------------------------------------------------------
using System;
using System.Data;
#if NETFX
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif
using System.Threading.Tasks;

namespace Csla.Data.SqlClient
{
  /// <summary>
  /// This is a SqlDataReader that 'fixes' any null values before
  /// they are returned to our business code.
  /// </summary>
  public class SafeSqlDataReader : SafeDataReader
  {
    /// <summary>
    /// Get a reference to the underlying
    /// SqlDataReader if present.
    /// </summary>
    public SqlDataReader SqlDataReader { get; }

    /// <summary>
    /// Initializes the SafeDataReader object to use data from
    /// the provided DataReader object.
    /// </summary>
    /// <param name="dataReader">The source DataReader object containing the data.</param>
    public SafeSqlDataReader(IDataReader dataReader)
      :base(dataReader)
    {
      SqlDataReader = DataReader as SqlDataReader;
    }

    /// <summary>
    /// Asynchronously gets the data value as a type.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <returns></returns>
    public Task<T> GetFieldValueAsync<T>(int ordinal)
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("GetFieldValueAsync");
      return SqlDataReader.GetFieldValueAsync<T>(ordinal);
    }

    /// <summary>
    /// Asynchronously gets the data value as a type.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <param name="cancellationToken">Async cancellation token</param>
    public Task<T> GetFieldValueAsync<T>(int ordinal, System.Threading.CancellationToken cancellationToken)
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("GetFieldValueAsync");
      return SqlDataReader.GetFieldValueAsync<T>(ordinal, cancellationToken);
    }

    /// <summary>
    /// Gets a value indicating whether the column has a null
    /// or missing value.
    /// </summary>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <returns></returns>
    public Task<bool> IsDbNullAsync(int ordinal)
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("IsDbNullAsync");
      return SqlDataReader.IsDBNullAsync(ordinal);
    }

    /// <summary>
    /// Gets a value indicating whether the column has a null
    /// or missing value.
    /// </summary>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <param name="cancellationToken">Async cancellation token</param>
    /// <returns></returns>
    public Task<bool> IsDbNullAsync(int ordinal, System.Threading.CancellationToken cancellationToken)
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("IsDbNullAsync");
      return SqlDataReader.IsDBNullAsync(ordinal, cancellationToken);
    }

    /// <summary>
    /// Advances the reader to the next result.
    /// </summary>
    /// <returns></returns>
    public Task<bool> NextResultAsync()
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return SqlDataReader.NextResultAsync();
    }

    /// <summary>
    /// Advances the reader to the next result.
    /// </summary>
    /// <param name="cancellationToken">Async cancellation token</param>
    /// <returns></returns>
    public Task<bool> NextResultAsync(System.Threading.CancellationToken cancellationToken)
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return SqlDataReader.NextResultAsync(cancellationToken);
    }

    /// <summary>
    /// Advances to the next record in a recordset.
    /// </summary>
    /// <returns></returns>
    public Task<bool> ReadAsync()
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return SqlDataReader.ReadAsync();
    }

    /// <summary>
    /// Advances to the next record in a recordset.
    /// </summary>
    /// <param name="cancellationToken">Async cancellation token</param>
    /// <returns></returns>
    public Task<bool> ReadAsync(System.Threading.CancellationToken cancellationToken)
    {
      if (SqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return SqlDataReader.ReadAsync(cancellationToken);
    }
  }
}