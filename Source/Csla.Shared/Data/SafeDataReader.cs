#if !NETFX_CORE && !IOS
//-----------------------------------------------------------------------
// <copyright file="SafeDataReader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is a DataReader that 'fixes' any null values before</summary>
//-----------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Csla.Data
{
  /// <summary>
  /// This is a DataReader that 'fixes' any null values before
  /// they are returned to our business code.
  /// </summary>
  public class SafeDataReader : IDataReader
  {
    private IDataReader _dataReader;
#if !NET40
    private SqlDataReader _sqlDataReader;
#endif

    /// <summary>
    /// Get a reference to the underlying data reader
    /// object that actually contains the data from
    /// the data source.
    /// </summary>
    protected IDataReader DataReader
    {
      get { return _dataReader; }
    }

    /// <summary>
    /// Initializes the SafeDataReader object to use data from
    /// the provided DataReader object.
    /// </summary>
    /// <param name="dataReader">The source DataReader object containing the data.</param>
    public SafeDataReader(IDataReader dataReader)
    {
      _dataReader = dataReader;
#if !NET40
      _sqlDataReader = _dataReader as SqlDataReader;
#endif
    }

#if !NET40
    /// <summary>
    /// Asynchronously gets the data value as a type.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <returns></returns>
    public Task<T> GetFieldValueAsync<T>(int ordinal)
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("GetFieldValueAsync");
      return _sqlDataReader.GetFieldValueAsync<T>(ordinal);
    }

    /// <summary>
    /// Asynchronously gets the data value as a type.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <param name="cancellationToken">Async cancellation token</param>
    public Task<T> GetFieldValueAsync<T>(int ordinal, System.Threading.CancellationToken cancellationToken)
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("GetFieldValueAsync");
      return _sqlDataReader.GetFieldValueAsync<T>(ordinal, cancellationToken);
    }

    /// <summary>
    /// Gets a value indicating whether the column has a null
    /// or missing value.
    /// </summary>
    /// <param name="ordinal">Ordinal position of value</param>
    /// <returns></returns>
    public Task<bool> IsDbNullAsync(int ordinal)
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("IsDbNullAsync");
      return _sqlDataReader.IsDBNullAsync(ordinal);
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
      if (_sqlDataReader == null)
        throw new NotSupportedException("IsDbNullAsync");
      return _sqlDataReader.IsDBNullAsync(ordinal, cancellationToken);
    }

    /// <summary>
    /// Advances the reader to the next result.
    /// </summary>
    /// <returns></returns>
    public Task<bool> NextResultAsync()
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return _sqlDataReader.NextResultAsync();
    }

    /// <summary>
    /// Advances the reader to the next result.
    /// </summary>
    /// <param name="cancellationToken">Async cancellation token</param>
    /// <returns></returns>
    public Task<bool> NextResultAsync(System.Threading.CancellationToken cancellationToken)
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return _sqlDataReader.NextResultAsync(cancellationToken);
    }

    /// <summary>
    /// Advances to the next record in a recordset.
    /// </summary>
    /// <returns></returns>
    public Task<bool> ReadAsync()
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return _sqlDataReader.ReadAsync();
    }

    /// <summary>
    /// Advances to the next record in a recordset.
    /// </summary>
    /// <param name="cancellationToken">Async cancellation token</param>
    /// <returns></returns>
    public Task<bool> ReadAsync(System.Threading.CancellationToken cancellationToken)
    {
      if (_sqlDataReader == null)
        throw new NotSupportedException("NextResultAsync");
      return _sqlDataReader.ReadAsync(cancellationToken);
    }
#endif

    /// <summary>
    /// Gets a string value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns empty string for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public string GetString(string name)
    {
      return GetString(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a string value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns empty string for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual string GetString(int i)
    {
      if (_dataReader.IsDBNull(i))
        return string.Empty;
      else
        return _dataReader.GetString(i);
    }


    /// <summary>
    /// Gets a value of type <see cref="System.Object" /> from the datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public object GetValue(string name)
    {
      return GetValue(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a value of type <see cref="System.Object" /> from the datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual object GetValue(int i)
    {
      if (_dataReader.IsDBNull(i))
        return null;
      else
        return _dataReader.GetValue(i);
    }

    /// <summary>
    /// Gets an integer from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public int GetInt32(string name)
    {
      return GetInt32(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets an integer from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual int GetInt32(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetInt32(i);
    }

    /// <summary>
    /// Gets a double from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public double GetDouble(string name)
    {
      return GetDouble(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a double from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual double GetDouble(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetDouble(i);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate" /> from the datareader.
    /// </summary>
    /// <remarks>
    /// A null is converted into min possible date
    /// See Chapter 5 for more details on the SmartDate class.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public Csla.SmartDate GetSmartDate(string name)
    {
      return GetSmartDate(_dataReader.GetOrdinal(name), true);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate" /> from the datareader.
    /// </summary>
    /// <remarks>
    /// A null is converted into the min possible date
    /// See Chapter 5 for more details on the SmartDate class.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual Csla.SmartDate GetSmartDate(int i)
    {
      return GetSmartDate(i, true);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate" /> from the datareader.
    /// </summary>
    /// <remarks>
    /// A null is converted into either the min or max possible date
    /// depending on the MinIsEmpty parameter. See Chapter 5 for more
    /// details on the SmartDate class.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <param name="minIsEmpty">
    /// A flag indicating whether the min or max 
    /// value of a data means an empty date.</param>
    public Csla.SmartDate GetSmartDate(string name, bool minIsEmpty)
    {
      return GetSmartDate(_dataReader.GetOrdinal(name), minIsEmpty);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate"/> from the datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    /// <param name="minIsEmpty">
    /// A flag indicating whether the min or max 
    /// value of a data means an empty date.</param>
    public virtual Csla.SmartDate GetSmartDate(
      int i, bool minIsEmpty)
    {
      if (_dataReader.IsDBNull(i))
        return new Csla.SmartDate(minIsEmpty);
      else
        return new Csla.SmartDate(
          _dataReader.GetDateTime(i), minIsEmpty);
    }

    /// <summary>
    /// Gets a Guid value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Guid.Empty for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public System.Guid GetGuid(string name)
    {
      return GetGuid(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a Guid value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Guid.Empty for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual System.Guid GetGuid(int i)
    {
      if (_dataReader.IsDBNull(i))
        return Guid.Empty;
      else
        return _dataReader.GetGuid(i);
    }

    /// <summary>
    /// Reads the next row of data from the datareader.
    /// </summary>
    public bool Read()
    {
      return _dataReader.Read();
    }

    /// <summary>
    /// Moves to the next result set in the datareader.
    /// </summary>
    public bool NextResult()
    {
      return _dataReader.NextResult();
    }

    /// <summary>
    /// Closes the datareader.
    /// </summary>
    public void Close()
    {
      _dataReader.Close();
    }

    /// <summary>
    /// Returns the depth property value from the datareader.
    /// </summary>
    public int Depth
    {
      get
      {
        return _dataReader.Depth;
      }
    }

    /// <summary>
    /// Returns the FieldCount property from the datareader.
    /// </summary>
    public int FieldCount
    {
      get
      {
        return _dataReader.FieldCount;
      }
    }

    /// <summary>
    /// Gets a boolean value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns false for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public bool GetBoolean(string name)
    {
      return GetBoolean(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a boolean value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns false for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual bool GetBoolean(int i)
    {
      if (_dataReader.IsDBNull(i))
        return false;
      else
        return _dataReader.GetBoolean(i);
    }

    /// <summary>
    /// Gets a byte value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public byte GetByte(string name)
    {
      return GetByte(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a byte value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual byte GetByte(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetByte(i);
    }

    /// <summary>
    /// Invokes the GetBytes method of the underlying datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <param name="buffer">Array containing the data.</param>
    /// <param name="bufferOffset">Offset position within the buffer.</param>
    /// <param name="fieldOffset">Offset position within the field.</param>
    /// <param name="length">Length of data to read.</param>
    public Int64 GetBytes(string name, Int64 fieldOffset,
      byte[] buffer, int bufferOffset, int length)
    {
      return GetBytes(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Invokes the GetBytes method of the underlying datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    /// <param name="buffer">Array containing the data.</param>
    /// <param name="bufferOffset">Offset position within the buffer.</param>
    /// <param name="fieldOffset">Offset position within the field.</param>
    /// <param name="length">Length of data to read.</param>
    public virtual Int64 GetBytes(int i, Int64 fieldOffset,
      byte[] buffer, int bufferOffset, int length)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Gets a char value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Char.MinValue for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public char GetChar(string name)
    {
      return GetChar(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a char value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Char.MinValue for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual char GetChar(int i)
    {
      if (_dataReader.IsDBNull(i))
        return char.MinValue;
      else
      {
        char[] myChar = new char[1];
        _dataReader.GetChars(i, 0, myChar, 0, 1);
        return myChar[0];
      }
    }

    /// <summary>
    /// Invokes the GetChars method of the underlying datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <param name="buffer">Array containing the data.</param>
    /// <param name="bufferOffset">Offset position within the buffer.</param>
    /// <param name="fieldOffset">Offset position within the field.</param>
    /// <param name="length">Length of data to read.</param>
    public Int64 GetChars(string name, Int64 fieldOffset,
      char[] buffer, int bufferOffset, int length)
    {
      return GetChars(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Invokes the GetChars method of the underlying datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    /// <param name="buffer">Array containing the data.</param>
    /// <param name="bufferOffset">Offset position within the buffer.</param>
    /// <param name="fieldOffset">Offset position within the field.</param>
    /// <param name="length">Length of data to read.</param>
    public virtual Int64 GetChars(int i, Int64 fieldOffset,
      char[] buffer, int bufferOffset, int length)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetChars(i, fieldOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Invokes the GetData method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public IDataReader GetData(string name)
    {
      return GetData(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Invokes the GetData method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual IDataReader GetData(int i)
    {
      return _dataReader.GetData(i);
    }

    /// <summary>
    /// Invokes the GetDataTypeName method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public string GetDataTypeName(string name)
    {
      return GetDataTypeName(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Invokes the GetDataTypeName method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual string GetDataTypeName(int i)
    {
      return _dataReader.GetDataTypeName(i);
    }

    /// <summary>
    /// Gets a date value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns DateTime.MinValue for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public virtual DateTime GetDateTime(string name)
    {
      return GetDateTime(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a date value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns DateTime.MinValue for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual DateTime GetDateTime(int i)
    {
      if (_dataReader.IsDBNull(i))
        return DateTime.MinValue;
      else
        return _dataReader.GetDateTime(i);
    }

    /// <summary>
    /// Gets an UTC date value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns DateTimeOffset.MinValue for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public virtual DateTimeOffset GetDateTimeOffset(string name)
    {
      return GetDateTimeOffset(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets an UTC date value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns DateTimeOffset.MinValue for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual DateTimeOffset GetDateTimeOffset(int i)
    {
      if (_dataReader.IsDBNull(i))
        return DateTimeOffset.MinValue;
      else
        return (DateTimeOffset)_dataReader.GetValue(i);
    }

    /// <summary>
    /// Gets a decimal value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public decimal GetDecimal(string name)
    {
      return GetDecimal(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a decimal value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual decimal GetDecimal(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetDecimal(i);
    }

    /// <summary>
    /// Invokes the GetFieldType method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public Type GetFieldType(string name)
    {
      return GetFieldType(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Invokes the GetFieldType method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual Type GetFieldType(int i)
    {
      return _dataReader.GetFieldType(i);
    }

    /// <summary>
    /// Gets a Single value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public float GetFloat(string name)
    {
      return GetFloat(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a Single value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual float GetFloat(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetFloat(i);
    }

    /// <summary>
    /// Gets a Short value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public short GetInt16(string name)
    {
      return GetInt16(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a Short value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual short GetInt16(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetInt16(i);
    }

    /// <summary>
    /// Gets a Long value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    public Int64 GetInt64(string name)
    {
      return GetInt64(_dataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a Long value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual Int64 GetInt64(int i)
    {
      if (_dataReader.IsDBNull(i))
        return 0;
      else
        return _dataReader.GetInt64(i);
    }

    /// <summary>
    /// Invokes the GetName method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual string GetName(int i)
    {
      return _dataReader.GetName(i);
    }

    /// <summary>
    /// Gets an ordinal value from the datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public int GetOrdinal(string name)
    {
      return _dataReader.GetOrdinal(name);
    }

    /// <summary>
    /// Invokes the GetSchemaTable method of the underlying datareader.
    /// </summary>
    public DataTable GetSchemaTable()
    {
      return _dataReader.GetSchemaTable();
    }


    /// <summary>
    /// Invokes the GetValues method of the underlying datareader.
    /// </summary>
    /// <param name="values">An array of System.Object to
    /// copy the values into.</param>
    public int GetValues(object[] values)
    {
      return _dataReader.GetValues(values);
    }

    /// <summary>
    /// Returns the IsClosed property value from the datareader.
    /// </summary>
    public bool IsClosed
    {
      get
      {
        return _dataReader.IsClosed;
      }
    }

    /// <summary>
    /// Invokes the IsDBNull method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual bool IsDBNull(int i)
    {
      return _dataReader.IsDBNull(i);
    }

    /// <summary>
    /// Invokes the IsDBNull method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public virtual bool IsDBNull(string name)
    {
      int index = this.GetOrdinal(name);
      return this.IsDBNull(index);
    }

    /// <summary>
    /// Returns a value from the datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    public object this[string name]
    {
      get
      {
        object val = _dataReader[name];
        if (DBNull.Value.Equals(val))
          return null;
        else
          return val;
      }
    }

    /// <summary>
    /// Returns a value from the datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual object this[int i]
    {
      get
      {
        if (_dataReader.IsDBNull(i))
          return null;
        else
          return _dataReader[i];
      }
    }
    /// <summary>
    /// Returns the RecordsAffected property value from the underlying datareader.
    /// </summary>
    public int RecordsAffected
    {
      get
      {
        return _dataReader.RecordsAffected;
      }
    }

    #region IDisposable Support

    private bool _disposedValue; // To detect redundant calls

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">True if called by
    /// the public Dispose method.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!_disposedValue)
      {
        if (disposing)
        {
          // free unmanaged resources when explicitly called
          _dataReader.Dispose();
        }

        // free shared unmanaged resources
      }
      _disposedValue = true;
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Object finalizer.
    /// </summary>
    ~SafeDataReader()
    {
      Dispose(false);
    }

    #endregion

  }
}
#endif