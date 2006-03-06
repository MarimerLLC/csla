Imports System.Data

Namespace Data

  ''' <summary>
  ''' This is a DataReader that 'fixes' any null values before
  ''' they are returned to our business code.
  ''' </summary>
  Public Class SafeDataReader

    Implements IDataReader

    Private mDataReader As IDataReader

    ''' <summary>
    ''' Get a reference to the underlying data reader
    ''' object that actually contains the data from
    ''' the data source.
    ''' </summary>
    Protected ReadOnly Property DataReader() As IDataReader
      Get
        Return mDataReader
      End Get
    End Property

    ''' <summary>
    ''' Initializes the SafeDataReader object to use data from
    ''' the provided DataReader object.
    ''' </summary>
    ''' <param name="DataReader">The source DataReader object containing the data.</param>
    Public Sub New(ByVal dataReader As IDataReader)
      mDataReader = dataReader
    End Sub

    ''' <summary>
    ''' Gets a string value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns empty string for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetString(ByVal i As Integer) As String _
      Implements IDataReader.GetString

      If mDataReader.IsDBNull(i) Then
        Return ""
      Else
        Return mDataReader.GetString(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a string value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns empty string for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetString(ByVal name As String) As String
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetString(index)
    End Function

    ''' <summary>
    ''' Gets a value of type <see cref="System.Object" /> from the datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetValue(ByVal i As Integer) As Object Implements IDataReader.GetValue
      If mDataReader.IsDBNull(i) Then
        Return Nothing
      Else
        Return mDataReader.GetValue(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a value of type <see cref="System.Object" /> from the datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetValue(ByVal name As String) As Object
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetValue(index)
    End Function

    ''' <summary>
    ''' Gets an integer from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetInt32(ByVal i As Integer) As Integer Implements IDataReader.GetInt32
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetInt32(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetInt32(index)
    End Function

    ''' <summary>
    ''' Gets a double from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDouble(ByVal i As Integer) As Double Implements IDataReader.GetDouble
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetDouble(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetDouble(index)
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
    ''' <param name="i">Ordinal column position of the value.</param>
    ''' <param name="MinIsEmpty">
    ''' A flag indicating whether the min or max value 
    ''' of a data means an empty date.</param>
    Public Overridable Function GetSmartDate( _
      ByVal i As Integer, ByVal minIsEmpty As Boolean) As SmartDate

      If mDataReader.IsDBNull(i) Then
        Return New SmartDate(minIsEmpty)

      Else
        Return New SmartDate(mDataReader.GetDateTime(i), minIsEmpty)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetSmartDate(index, True)
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
    Public Function GetSmartDate( _
      ByVal name As String, ByVal minIsEmpty As Boolean) As SmartDate

      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetSmartDate(index, minIsEmpty)
    End Function

    ''' <summary>
    ''' Gets a Guid value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns Guid.Empty for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetGuid(ByVal i As Integer) As Guid Implements IDataReader.GetGuid
      If mDataReader.IsDBNull(i) Then
        Return Guid.Empty
      Else
        Return mDataReader.GetGuid(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetGuid(index)
    End Function

    ''' <summary>
    ''' Reads the next row of data from the datareader.
    ''' </summary>
    Public Function Read() As Boolean Implements IDataReader.Read
      Return mDataReader.Read
    End Function

    ''' <summary>
    ''' Moves to the next result set in the datareader.
    ''' </summary>
    Public Function NextResult() As Boolean Implements IDataReader.NextResult
      Return mDataReader.NextResult()
    End Function

    ''' <summary>
    ''' Closes the datareader.
    ''' </summary>
    Public Sub Close() Implements IDataReader.Close
      mDataReader.Close()
    End Sub

    ''' <summary>
    ''' Returns the depth property value from the datareader.
    ''' </summary>
    Public ReadOnly Property Depth() As Integer Implements System.Data.IDataReader.Depth
      Get
        Return mDataReader.Depth
      End Get
    End Property

    ''' <summary>
    ''' Returns the FieldCount property from the datareader.
    ''' </summary>
    Public ReadOnly Property FieldCount() As Integer Implements System.Data.IDataReader.FieldCount
      Get
        Return mDataReader.FieldCount
      End Get
    End Property

    ''' <summary>
    ''' Gets a boolean value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns <see langword="false" /> for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetBoolean(ByVal i As Integer) As Boolean _
      Implements System.Data.IDataReader.GetBoolean

      If mDataReader.IsDBNull(i) Then
        Return False
      Else
        Return mDataReader.GetBoolean(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a boolean value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns <see langword="false" /> for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetBoolean(ByVal name As String) As Boolean
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetBoolean(index)
    End Function

    ''' <summary>
    ''' Gets a byte value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetByte(ByVal i As Integer) As Byte Implements System.Data.IDataReader.GetByte
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetByte(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetByte(index)
    End Function

    ''' <summary>
    ''' Invokes the GetBytes method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetBytes(ByVal i As Integer, ByVal fieldOffset As Long, ByVal buffer() As Byte, ByVal bufferoffset As Integer, ByVal length As Integer) As Long Implements System.Data.IDataReader.GetBytes
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetBytes method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetBytes(ByVal name As String, ByVal fieldOffset As Long, ByVal buffer() As Byte, ByVal bufferoffset As Integer, ByVal length As Integer) As Long
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetBytes(index, fieldOffset, buffer, bufferoffset, length)
    End Function

    ''' <summary>
    ''' Gets a char value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns Char.MinValue for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetChar(ByVal i As Integer) As Char Implements System.Data.IDataReader.GetChar
      If mDataReader.IsDBNull(i) Then
        Return Char.MinValue
      Else
        Dim myChar(0) As Char
        mDataReader.GetChars(i, 0, myChar, 0, 1)
        Return myChar(0)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetChar(index)
    End Function

    ''' <summary>
    ''' Invokes the GetChars method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetChars(ByVal i As Integer, ByVal fieldoffset As Long, ByVal buffer() As Char, ByVal bufferoffset As Integer, ByVal length As Integer) As Long Implements System.Data.IDataReader.GetChars
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
      End If
    End Function

    ''' <summary>
    ''' Invokes the GetChars method of the underlying datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetChars(ByVal name As String, ByVal fieldoffset As Long, ByVal buffer() As Char, ByVal bufferoffset As Integer, ByVal length As Integer) As Long
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetChars(index, fieldoffset, buffer, bufferoffset, length)
    End Function

    ''' <summary>
    ''' Invokes the GetData method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetData(ByVal i As Integer) As System.Data.IDataReader Implements System.Data.IDataReader.GetData
      Return mDataReader.GetData(i)
    End Function

    ''' <summary>
    ''' Invokes the GetData method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetData(ByVal name As String) As System.Data.IDataReader
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetData(index)
    End Function

    ''' <summary>
    ''' Invokes the GetDataTypeName method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDataTypeName(ByVal i As Integer) As String Implements System.Data.IDataReader.GetDataTypeName
      Return mDataReader.GetDataTypeName(i)
    End Function

    ''' <summary>
    ''' Invokes the GetDataTypeName method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetDataTypeName(ByVal name As String) As String
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetDataTypeName(index)
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

      If mDataReader.IsDBNull(i) Then
        Return Date.MinValue
      Else
        Return mDataReader.GetDateTime(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a date value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns DateTime.MinValue for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetDateTime(ByVal name As String) As Date
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetDateTime(index)
    End Function

    ''' <summary>
    ''' Gets a decimal value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetDecimal(ByVal i As Integer) As Decimal Implements System.Data.IDataReader.GetDecimal
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetDecimal(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetDecimal(index)
    End Function

    ''' <summary>
    ''' Invokes the GetFieldType method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetFieldType(ByVal i As Integer) As System.Type Implements System.Data.IDataReader.GetFieldType
      Return mDataReader.GetFieldType(i)
    End Function

    ''' <summary>
    ''' Invokes the GetFieldType method of the underlying datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetFieldType(ByVal name As String) As System.Type
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetFieldType(index)
    End Function

    ''' <summary>
    ''' Gets a Single value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetFloat(ByVal i As Integer) As Single Implements System.Data.IDataReader.GetFloat
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetFloat(i)
      End If
    End Function

    ''' <summary>
    ''' Gets a Single value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetFloat(ByVal name As String) As Single
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetFloat(index)
    End Function

    ''' <summary>
    ''' Gets a Short value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetInt16(ByVal i As Integer) As Short Implements System.Data.IDataReader.GetInt16
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetInt16(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetInt16(index)
    End Function

    ''' <summary>
    ''' Gets a Long value from the datareader.
    ''' </summary>
    ''' <remarks>
    ''' Returns 0 for null.
    ''' </remarks>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetInt64(ByVal i As Integer) As Long Implements System.Data.IDataReader.GetInt64
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetInt64(i)
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
      Dim index As Integer = Me.GetOrdinal(name)
      Return Me.GetInt64(index)
    End Function

    ''' <summary>
    ''' Invokes the GetName method of the underlying datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Public Overridable Function GetName(ByVal i As Integer) As String Implements System.Data.IDataReader.GetName
      Return mDataReader.GetName(i)
    End Function

    ''' <summary>
    ''' Gets an ordinal value from the datareader.
    ''' </summary>
    ''' <param name="name">Name of the column containing the value.</param>
    Public Function GetOrdinal(ByVal name As String) As Integer _
      Implements System.Data.IDataReader.GetOrdinal

      Return mDataReader.GetOrdinal(name)
    End Function

    ''' <summary>
    ''' Invokes the GetSchemaTable method of the underlying datareader.
    ''' </summary>
    Public Function GetSchemaTable() As System.Data.DataTable Implements System.Data.IDataReader.GetSchemaTable
      Return mDataReader.GetSchemaTable
    End Function

    ''' <summary>
    ''' Invokes the GetValues method of the underlying datareader.
    ''' </summary>
    Public Function GetValues(ByVal values() As Object) As Integer Implements System.Data.IDataReader.GetValues
      Return mDataReader.GetValues(values)
    End Function

    ''' <summary>
    ''' Returns the IsClosed property value from the datareader.
    ''' </summary>
    Public ReadOnly Property IsClosed() As Boolean Implements System.Data.IDataReader.IsClosed
      Get
        Return mDataReader.IsClosed
      End Get
    End Property

    ''' <summary>
    ''' Invokes the IsDBNull method of the underlying datareader.
    ''' </summary>
    Public Overridable Function IsDBNull(ByVal i As Integer) As Boolean Implements System.Data.IDataReader.IsDBNull
      Return mDataReader.IsDBNull(i)
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
        Dim value As Object = mDataReader.Item(name)
        If DBNull.Value.Equals(value) Then
          Return Nothing
        Else
          Return value
        End If
      End Get
    End Property

    ''' <summary>
    ''' Returns a value from the datareader.
    ''' </summary>
    ''' <param name="i">Ordinal column position of the value.</param>
    Default Public Overridable Overloads ReadOnly Property Item(ByVal i As Integer) As Object Implements System.Data.IDataReader.Item
      Get
        If mDataReader.IsDBNull(i) Then
          Return Nothing
        Else
          Return mDataReader.Item(i)
        End If
      End Get
    End Property

    ''' <summary>
    ''' Returns the RecordsAffected property value from the underlying datareader.
    ''' </summary>
    Public ReadOnly Property RecordsAffected() As Integer Implements System.Data.IDataReader.RecordsAffected
      Get
        Return mDataReader.RecordsAffected
      End Get
    End Property

#Region " IDisposable Support "

    Private disposedValue As Boolean     ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If Not Me.disposedValue Then
        If disposing Then
          ' free unmanaged resources when explicitly called
          mDataReader.Dispose()
        End If

        ' free shared unmanaged resources
      End If
      Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
      ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
      Dispose(False)
    End Sub

#End Region

  End Class

End Namespace
