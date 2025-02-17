//-----------------------------------------------------------------------
// <copyright file="SafeDataReader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is a DataReader that 'fixes' any null values before</summary>
//-----------------------------------------------------------------------

using System.Data;
using System.Diagnostics.CodeAnalysis;

#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
using System.Data.SqlClient;
#endif

namespace Csla.Data
{
  /// <summary>
  /// This is an IDataReader that 'fixes' any null values before
  /// they are returned to our business code.
  /// </summary>
  public class SafeDataReader : IDataReader
  {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
    private SqlDataReader? _sqlDataReader;
#endif

    /// <summary>
    /// Get a reference to the underlying data reader
    /// object that actually contains the data from
    /// the data source.
    /// </summary>
    protected IDataReader DataReader { get; }

    /// <summary>
    /// Initializes the SafeDataReader object to use data from
    /// the provided DataReader object.
    /// </summary>
    /// <param name="dataReader">The source DataReader object containing the data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dataReader"/> is <see langword="null"/>.</exception>
    public SafeDataReader(IDataReader dataReader)
    {
      DataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
      _sqlDataReader = DataReader as SqlDataReader;
#endif
    }

    /// <summary>
    /// Gets a string value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns empty string for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public string GetString(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetString(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return string.Empty;
      else
        return DataReader.GetString(i);
    }


    /// <summary>
    /// Gets a value of type <see cref="System.Object" /> from the datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object? GetValue(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetValue(DataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a value of type <see cref="System.Object" /> from the datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes). We are suppressing it because we do return null
    public virtual object? GetValue(int i)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
      if (DataReader.IsDBNull(i))
        return null;
      else
        return DataReader.GetValue(i);
    }

    /// <summary>
    /// Gets an integer from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public int GetInt32(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetInt32(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetInt32(i);
    }

    /// <summary>
    /// Gets a double from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public double GetDouble(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetDouble(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetDouble(i);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate" /> from the datareader.
    /// </summary>
    /// <remarks>
    /// A null is converted into min possible date
    /// See Chapter 5 for more details on the SmartDate class.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public SmartDate GetSmartDate(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetSmartDate(DataReader.GetOrdinal(name), true);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate" /> from the datareader.
    /// </summary>
    /// <remarks>
    /// A null is converted into the min possible date
    /// See Chapter 5 for more details on the SmartDate class.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual SmartDate GetSmartDate(int i)
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
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public SmartDate GetSmartDate(string name, bool minIsEmpty)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetSmartDate(DataReader.GetOrdinal(name), minIsEmpty);
    }

    /// <summary>
    /// Gets a <see cref="SmartDate"/> from the datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    /// <param name="minIsEmpty">
    /// A flag indicating whether the min or max 
    /// value of a data means an empty date.</param>
    public virtual SmartDate GetSmartDate(int i, bool minIsEmpty)
    {
      if (DataReader.IsDBNull(i))
        return new SmartDate(minIsEmpty);
      else
        return new SmartDate(
          DataReader.GetDateTime(i), minIsEmpty);
    }

    /// <summary>
    /// Gets a Guid value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Guid.Empty for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Guid GetGuid(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetGuid(DataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Gets a Guid value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Guid.Empty for null.
    /// </remarks>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual Guid GetGuid(int i)
    {
      if (DataReader.IsDBNull(i))
        return Guid.Empty;
      else
        return DataReader.GetGuid(i);
    }

    /// <summary>
    /// Reads the next row of data from the datareader.
    /// </summary>
    public bool Read()
    {
      return DataReader.Read();
    }

    /// <summary>
    /// Moves to the next result set in the datareader.
    /// </summary>
    public bool NextResult()
    {
      return DataReader.NextResult();
    }

    /// <summary>
    /// Closes the datareader.
    /// </summary>
    public void Close()
    {
      DataReader.Close();
    }

    /// <summary>
    /// Returns the depth property value from the datareader.
    /// </summary>
    public int Depth
    {
      get
      {
        return DataReader.Depth;
      }
    }

    /// <summary>
    /// Returns the FieldCount property from the datareader.
    /// </summary>
    public int FieldCount
    {
      get
      {
        return DataReader.FieldCount;
      }
    }

    /// <summary>
    /// Gets a boolean value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns false for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public bool GetBoolean(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetBoolean(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return false;
      else
        return DataReader.GetBoolean(i);
    }

    /// <summary>
    /// Gets a byte value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public byte GetByte(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetByte(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetByte(i);
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
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Int64 GetBytes(string name, Int64 fieldOffset, byte[] buffer, int bufferOffset, int length)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetBytes(DataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
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
    public virtual Int64 GetBytes(int i, Int64 fieldOffset, byte[]? buffer, int bufferOffset, int length)
    {
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Gets a char value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns Char.MinValue for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public char GetChar(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetChar(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return char.MinValue;
      else
      {
        char[] myChar = new char[1];
        DataReader.GetChars(i, 0, myChar, 0, 1);
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
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Int64 GetChars(string name, Int64 fieldOffset, char[] buffer, int bufferOffset, int length)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetChars(DataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
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
    public virtual Int64 GetChars(int i, Int64 fieldOffset, char[]? buffer, int bufferOffset, int length)
    {
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetChars(i, fieldOffset, buffer, bufferOffset, length);
    }

    /// <summary>
    /// Invokes the GetData method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public IDataReader GetData(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetData(DataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Invokes the GetData method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual IDataReader GetData(int i)
    {
      return DataReader.GetData(i);
    }

    /// <summary>
    /// Invokes the GetDataTypeName method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public string GetDataTypeName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetDataTypeName(DataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Invokes the GetDataTypeName method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual string GetDataTypeName(int i)
    {
      return DataReader.GetDataTypeName(i);
    }

    /// <summary>
    /// Gets a date value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns DateTime.MinValue for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public virtual DateTime GetDateTime(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetDateTime(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return DateTime.MinValue;
      else
        return DataReader.GetDateTime(i);
    }

    /// <summary>
    /// Gets an UTC date value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns DateTimeOffset.MinValue for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public virtual DateTimeOffset GetDateTimeOffset(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetDateTimeOffset(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return DateTimeOffset.MinValue;
      else
        return (DateTimeOffset)DataReader.GetValue(i);
    }

    /// <summary>
    /// Gets a decimal value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public decimal GetDecimal(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetDecimal(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetDecimal(i);
    }

    /// <summary>
    /// Invokes the GetFieldType method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Type GetFieldType(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetFieldType(DataReader.GetOrdinal(name));
    }

    /// <summary>
    /// Invokes the GetFieldType method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
    public virtual Type GetFieldType(int i)
    {
      return DataReader.GetFieldType(i);
    }

    /// <summary>
    /// Gets a Single value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public float GetFloat(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetFloat(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetFloat(i);
    }

    /// <summary>
    /// Gets a Short value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public short GetInt16(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetInt16(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetInt16(i);
    }

    /// <summary>
    /// Gets a Long value from the datareader.
    /// </summary>
    /// <remarks>
    /// Returns 0 for null.
    /// </remarks>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public Int64 GetInt64(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return GetInt64(DataReader.GetOrdinal(name));
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
      if (DataReader.IsDBNull(i))
        return 0;
      else
        return DataReader.GetInt64(i);
    }

    /// <summary>
    /// Invokes the GetName method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual string GetName(int i)
    {
      return DataReader.GetName(i);
    }

    /// <summary>
    /// Gets an ordinal value from the datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public int GetOrdinal(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      return DataReader.GetOrdinal(name);
    }

    /// <summary>
    /// Invokes the GetSchemaTable method of the underlying datareader.
    /// </summary>
    public DataTable? GetSchemaTable()
    {
      return DataReader.GetSchemaTable();
    }


    /// <summary>
    /// Invokes the GetValues method of the underlying datareader.
    /// </summary>
    /// <param name="values">An array of System.Object to
    /// copy the values into.</param>
    public int GetValues(object[] values)
    {
      return DataReader.GetValues(values);
    }

    /// <summary>
    /// Returns the IsClosed property value from the datareader.
    /// </summary>
    public bool IsClosed
    {
      get
      {
        return DataReader.IsClosed;
      }
    }

    /// <summary>
    /// Invokes the IsDBNull method of the underlying datareader.
    /// </summary>
    /// <param name="i">Ordinal column position of the value.</param>
    public virtual bool IsDBNull(int i)
    {
      return DataReader.IsDBNull(i);
    }

    /// <summary>
    /// Invokes the IsDBNull method of the underlying datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public virtual bool IsDBNull(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

      int index = GetOrdinal(name);
      return IsDBNull(index);
    }

    /// <summary>
    /// Returns a value from the datareader.
    /// </summary>
    /// <param name="name">Name of the column containing the value.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object? this[string name]
    {
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes). We are suppressing it because we do return null
      get
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
      {
        if (string.IsNullOrWhiteSpace(name))
          throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(name)), nameof(name));

        object? val = DataReader[name];
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
    public virtual object? this[int i]
    {
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes). We are suppressing it because we do return null
      get
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
      {
        if (DataReader.IsDBNull(i))
          return null;
        else
          return DataReader[i];
      }
    }
    /// <summary>
    /// Returns the RecordsAffected property value from the underlying datareader.
    /// </summary>
    public int RecordsAffected
    {
      get
      {
        return DataReader.RecordsAffected;
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
          DataReader.Dispose();
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
