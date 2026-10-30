//-----------------------------------------------------------------------
// <copyright file="SafeSqlDataReader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is a SqlDataReader based on SafeDataReader</summary>
//-----------------------------------------------------------------------

using System.Data;
#if NETFX
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif

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
    public SqlDataReader? SqlDataReader { get; }

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
    /// Gets a value indicating whether the column has a null
    /// or missing value.
    /// </summary>
    /// <param name="ordinal">Ordinal position of value</param>
    [Obsolete("Use IsDBNullAsync")]
    public Task<bool> IsDbNullAsync(int ordinal)
    {
      return IsDBNullAsync(ordinal);
    }

    /// <summary>
    /// Gets a value indicating whether the column has a null
    /// or missing value.
    /// </summary>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <param name="cancellationToken">Async cancellation token</param>
    [Obsolete("Use IsDBNullAsync")]
    public Task<bool> IsDbNullAsync(int ordinal, CancellationToken cancellationToken)
    {
      return IsDBNullAsync(ordinal, cancellationToken);
    }
  }
}