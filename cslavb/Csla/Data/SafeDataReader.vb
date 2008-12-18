Imports System
Imports System.Data

Namespace Data

  ''' <summary>
  ''' This is a DataReader that 'fixes' any null values before
  ''' they are returned to our business code.
  ''' </summary>
  Public Class SafeDataReader

    Implements IDataReader

    Private _dataReader As IDataReader

    ''' <summary>
    ''' Get a reference to the underlying data reader
    ''' object that actually contains the data from
    ''' the data source.
    ''' </summary>
    Protected ReadOnly Property DataReader() As IDataReader
      Get
        Return _dataReader
      End Get
    End Property

    ''' <summary>
    ''' Initializes the SafeDataReader object to use data from
    ''' the provided DataReader object.
    ''' </summary>
    ''' <param name="dataReader">The source DataReader object containing the data.</param>
    Public Sub New(ByVal dataReader As IDataReader)
      _dataReader = dataReader
    End Sub

    ''' <summary>
    ''' Gets a string value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns empty string for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetString(ByVal name As String) As String
      Return GetString(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a string value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns empty string for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetString(ByVal i As Integer) As String _
      Implements IDataReader.GetString

      If _dataReader.IsDBNull(i) Then
        Return String.Empty
      Else
        Return _dataReader.GetString(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a value of type <see cref="System.Object" /> from the datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetValue(ByVal name As String) As Object
      Return GetValue(_dataReader.GetOrdinal(name))
    End Function
 
    ''' <summary>
    ''' Gets a value of type <see cref="System.Object" /> from the datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetValue(ByVal i As Integer) As Object Implements IDataReader.GetValue
      If _dataReader.IsDBNull(i) Then
        Return Nothing
      Else
        Return _dataReader.GetValue(i)
      End If
    End Function

    ''' <summary>
    ''' Gets an integer from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetInt32(ByVal name As String) As Integer
      Return GetInt32(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets an integer from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetInt32(ByVal i As Integer) As Integer Implements IDataReader.GetInt32
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetInt32(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a double from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetDouble(ByVal name As String) As Double
      Return GetDouble(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a double from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDouble(ByVal i As Integer) As Double Implements IDataReader.GetDouble
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetDouble(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a <see cref="SmartDate" /> from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' A null is converted into min possible date
    ''' See Chapter 5 for more details on the SmartDate class.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetSmartDate(ByVal name As String) As SmartDate
      Return GetSmartDate(_dataReader.GetOrdinal(name), True)
    End Function

    ''' <summary>
    ''' Gets a <see cref="SmartDate" /> from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' A null is converted into the min possible date
    ''' See Chapter 5 for more details on the SmartDate class.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetSmartDate(ByVal i As Integer) As SmartDate

      Return GetSmartDate(i, True)

    End Function

    ''' <summary>
    ''' Gets a <see cref="SmartDate" /> from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' A null is converted into either the min or max possible date
    ''' depending on the MinIsEmpty parameter. See Chapter 5 for more
    ''' details on the SmartDate class.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    ''' <param name="minIsEmpty">
    ''' A flag indicating whether the min or max 
    ''' value of a data means an empty date.</param>
    Public Function GetSmartDate(ByVal name As String, ByVal minIsEmpty As Boolean) As SmartDate
      Return GetSmartDate(_dataReader.GetOrdinal(name), minIsEmpty)
    End Function

    ''' <summary>
    ''' Gets a <see cref="SmartDate" /> from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' A null is converted into either the min or max possible date
    ''' depending on the MinIsEmpty parameter. See Chapter 5 for more
    ''' details on the SmartDate class.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    ''' <param name="minIsEmpty">
    ''' A flag indicating whether the min or max value 
    ''' of a data means an empty date.</param>
    Public Overridable Function GetSmartDate( _
      ByVal i As Integer, ByVal minIsEmpty As Boolean) As SmartDate

      If _dataReader.IsDBNull(i) Then
        Return New SmartDate(minIsEmpty)

      Else
        Return New SmartDate(_dataReader.GetDateTime(i), minIsEmpty)
      End If
    End Function

    ''' <summary>
    ''' Gets a Guid value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns Guid.Empty for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetGuid(ByVal name As String) As Guid
      Return GetGuid(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a Guid value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns Guid.Empty for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetGuid(ByVal i As Integer) As Guid Implements IDataReader.GetGuid
      If _dataReader.IsDBNull(i) Then
        Return Guid.Empty
      Else
        Return _dataReader.GetGuid(i)
      End If
    End Function

    ''' <summary>
    ''' Reads the next row of data from the datareader.
    ''' </summary>
    Public Function Read() As Boolean Implements IDataReader.Read
      Return _dataReader.Read()
    End Function

    ''' <summary>
    ''' Moves to the next result set in the datareader.
    ''' </summary>
    Public Function NextResult() As Boolean Implements IDataReader.NextResult
      Return _dataReader.NextResult()
    End Function

    ''' <summary>
    ''' Closes the datareader.
    ''' </summary>
    Public Sub Close() Implements IDataReader.Close
      _dataReader.Close()
    End Sub

    ''' <summary>
    ''' Returns the depth property value from the datareader.
    ''' </summary>
    Public ReadOnly Property Depth() As Integer Implements System.Data.IDataReader.Depth
      Get
        Return _dataReader.Depth
      End Get
    End Property

    ''' <summary>
    ''' Returns the FieldCount property from the datareader.
    ''' </summary>
    Public ReadOnly Property FieldCount() As Integer Implements System.Data.IDataReader.FieldCount
      Get
        Return _dataReader.FieldCount
      End Get
    End Property

    ''' <summary>
    ''' Gets a boolean value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns <see langword="false" /> for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetBoolean(ByVal name As String) As Boolean
      Return GetBoolean(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a boolean value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns <see langword="false" /> for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetBoolean(ByVal i As Integer) As Boolean _
      Implements System.Data.IDataReader.GetBoolean

      If _dataReader.IsDBNull(i) Then
        Return False
      Else
        Return _dataReader.GetBoolean(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a byte value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetByte(ByVal name As String) As Byte
      Return GetByte(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a byte value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetByte(ByVal i As Integer) As Byte Implements System.Data.IDataReader.GetByte
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetByte(i)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetBytes method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    ''' <param name="buffer">Array containing the data.</param>
    ''' <param name="bufferOffset">Offset position within the buffer.</param>
    ''' <param name="fieldOffset">Offset position within the field.</param>
    ''' <param name="length">Length of data to read.</param>
    Public Function GetBytes(ByVal name As String, ByVal fieldOffset As Long, ByVal buffer() As Byte, ByVal bufferOffset As Integer, ByVal length As Integer) As Long
      Return GetBytes(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length)
    End Function

    ''' <summary>
    ''' Invokes the GetBytes method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    ''' <param name="buffer">Array containing the data.</param>
    ''' <param name="bufferOffset">Offset position within the buffer.</param>
    ''' <param name="fieldOffset">Offset position within the field.</param>
    ''' <param name="length">Length of data to read.</param>
    Public Overridable Function GetBytes(ByVal i As Integer, ByVal fieldOffset As Long, ByVal buffer() As Byte, ByVal bufferOffset As Integer, ByVal length As Integer) As Long Implements System.Data.IDataReader.GetBytes
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetBytes(i, fieldOffset, buffer, bufferOffset, length)
      End If
    End Function

    ''' <summary>
    ''' Gets a char value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns Char.MinValue for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetChar(ByVal name As String) As Char
      Return GetChar(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a char value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns Char.MinValue for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetChar(ByVal i As Integer) As Char Implements System.Data.IDataReader.GetChar
      If _dataReader.IsDBNull(i) Then
        Return Char.MinValue
      Else
        Dim myChar(0) As Char
        _dataReader.GetChars(i, 0, myChar, 0, 1)
        Return myChar(0)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetChars method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    ''' <param name="buffer">Array containing the data.</param>
    ''' <param name="bufferOffset">Offset position within the buffer.</param>
    ''' <param name="fieldOffset">Offset position within the field.</param>
    ''' <param name="length">Length of data to read.</param>
    Public Function GetChars(ByVal name As String, ByVal fieldOffset As Long, ByVal buffer() As Char, ByVal bufferOffset As Integer, ByVal length As Integer) As Long
      Return GetChars(_dataReader.GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
    End Function

    ''' <summary>
    ''' Invokes the GetChars method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    ''' <param name="buffer">Array containing the data.</param>
    ''' <param name="bufferOffset">Offset position within the buffer.</param>
    ''' <param name="fieldOffset">Offset position within the field.</param>
    ''' <param name="length">Length of data to read.</param>
    Public Overridable Function GetChars(ByVal i As Integer, ByVal fieldOffset As Long, ByVal buffer() As Char, ByVal bufferOffset As Integer, ByVal length As Integer) As Long Implements System.Data.IDataReader.GetChars
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetChars(i, fieldOffset, buffer, bufferOffset, length)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetData method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetData(ByVal name As String) As System.Data.IDataReader
      Return GetData(_dataReader.GetOrdinal(name))
    End Function


    ''' <summary>
    ''' Invokes the GetData method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetData(ByVal i As Integer) As System.Data.IDataReader Implements System.Data.IDataReader.GetData
      Return _dataReader.GetData(i)
    End Function

    ''' <summary>
    ''' Invokes the GetDataTypeName method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetDataTypeName(ByVal name As String) As String
      Return GetDataTypeName(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Invokes the GetDataTypeName method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDataTypeName(ByVal i As Integer) As String Implements System.Data.IDataReader.GetDataTypeName
      Return _dataReader.GetDataTypeName(i)
    End Function

    ''' <summary>
    ''' Gets a date value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns DateTime.MinValue for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Overridable Function GetDateTime(ByVal name As String) As Date
      Return GetDateTime(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a date value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns DateTime.MinValue for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDateTime(ByVal i As Integer) As Date _
      Implements System.Data.IDataReader.GetDateTime

      If _dataReader.IsDBNull(i) Then
        Return Date.MinValue
      Else
        Return _dataReader.GetDateTime(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a decimal value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetDecimal(ByVal name As String) As Decimal
      Return GetDecimal(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a decimal value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDecimal(ByVal i As Integer) As Decimal Implements System.Data.IDataReader.GetDecimal
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetDecimal(i)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetFieldType method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetFieldType(ByVal name As String) As System.Type
      Return GetFieldType(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Invokes the GetFieldType method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetFieldType(ByVal i As Integer) As System.Type Implements System.Data.IDataReader.GetFieldType
      Return _dataReader.GetFieldType(i)
    End Function

    ''' <summary>
    ''' Gets a Single value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetFloat(ByVal name As String) As Single
      Return GetFloat(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a Single value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetFloat(ByVal i As Integer) As Single Implements System.Data.IDataReader.GetFloat
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetFloat(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a Short value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetInt16(ByVal name As String) As Short
      Return GetInt16(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a Short value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetInt16(ByVal i As Integer) As Short Implements System.Data.IDataReader.GetInt16
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetInt16(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a Long value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetInt64(ByVal name As String) As Long
      Return GetInt64(_dataReader.GetOrdinal(name))
    End Function

    ''' <summary>
    ''' Gets a Long value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetInt64(ByVal i As Integer) As Long Implements System.Data.IDataReader.GetInt64
      If _dataReader.IsDBNull(i) Then
        Return 0
      Else
        Return _dataReader.GetInt64(i)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetName method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetName(ByVal i As Integer) As String Implements System.Data.IDataReader.GetName
      Return _dataReader.GetName(i)
    End Function

    ''' <summary>
    ''' Gets an ordinal value from the datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetOrdinal(ByVal name As String) As Integer _
      Implements System.Data.IDataReader.GetOrdinal

      Return _dataReader.GetOrdinal(name)
    End Function

    ''' <summary>
    ''' Invokes the GetSchemaTable method of the underlying datareader.
    ''' </summary>
    Public Function GetSchemaTable() As System.Data.DataTable Implements System.Data.IDataReader.GetSchemaTable
      Return _dataReader.GetSchemaTable()
    End Function

    ''' <summary>
    ''' Invokes the GetValues method of the underlying datareader.
    ''' </summary>
    ''' <param name="values">An array of System.Object to
    ''' copy the values into.</param>
    Public Function GetValues(ByVal values() As Object) As Integer Implements System.Data.IDataReader.GetValues
      Return _dataReader.GetValues(values)
    End Function

    ''' <summary>
    ''' Returns the IsClosed property value from the datareader.
    ''' </summary>
    Public ReadOnly Property IsClosed() As Boolean Implements System.Data.IDataReader.IsClosed
      Get
        Return _dataReader.IsClosed
      End Get
    End Property

    ''' <summary>
    ''' Invokes the IsDBNull method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function IsDBNull(ByVal i As Integer) As Boolean Implements System.Data.IDataReader.IsDBNull
      Return _dataReader.IsDBNull(i)
    End Function

    ''' <summary>
    ''' Invokes the IsDBNull method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function IsDBNull(ByVal name As String) As Boolean
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.IsDBNull(index)
    End Function

    ''' <summary>
    ''' Returns a value from the datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Default Public Overloads ReadOnly Property Item(ByVal name As String) As Object Implements System.Data.IDataReader.Item
      Get
        Dim val As Object = _dataReader.Item(name)
        If DBNull.Value.Equals(val) Then
          Return Nothing
        Else
          Return val
        End If
      End Get
    End Property

    ''' <summary>
    ''' Returns a value from the datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Default Public Overridable Overloads ReadOnly Property Item(ByVal i As Integer) As Object Implements System.Data.IDataReader.Item
      Get
        If _dataReader.IsDBNull(i) Then
          Return Nothing
        Else
          Return _dataReader.Item(i)
        End If
      End Get
    End Property

    ''' <summary>
    ''' Returns the RecordsAffected property value from the underlying datareader.
    ''' </summary>
    Public ReadOnly Property RecordsAffected() As Integer Implements System.Data.IDataReader.RecordsAffected
      Get
        Return _dataReader.RecordsAffected
      End Get
    End Property

#Region " IDisposable Support "

    Private _disposedValue As Boolean     ' To detect redundant calls

    ''' <summary>
    ''' Disposes the object.
    ''' </summary>
    ''' <param name="disposing">True if called by
    ''' the public Dispose method.</param>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If Not Me._disposedValue Then
        If disposing Then
          ' free unmanaged resources when explicitly called
          _dataReader.Dispose()
        End If

        ' free shared unmanaged resources
      End If
      Me._disposedValue = True
    End Sub

    ''' <summary>
    ''' Disposes the object.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
      ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

    ''' <summary>
    ''' Object finalizer.
    ''' </summary>
    Protected Overrides Sub Finalize()
      Dispose(False)
    End Sub

#End Region

  End Class

End Namespace
