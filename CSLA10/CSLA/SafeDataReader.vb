Imports System.Data

Namespace Data

  Public Class SafeDataReader

    Implements IDataReader

    Private mDataReader As IDataReader

    Public Sub New(ByVal DataReader As IDataReader)
      mDataReader = DataReader
    End Sub

    Public Function GetString(ByVal i As Integer) As String Implements IDataReader.GetString
      If mDataReader.IsDBNull(i) Then
        Return ""
      Else
        Return mDataReader.GetString(i)
      End If
    End Function

    Public Function GetValue(ByVal i As Integer) As Object Implements IDataReader.GetValue
      If mDataReader.IsDBNull(i) Then
        Return Nothing
      Else
        Return mDataReader.GetValue(i)
      End If
    End Function

    Public Function GetInt32(ByVal i As Integer) As Integer Implements IDataReader.GetInt32
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetInt32(i)
      End If
    End Function

    Public Function GetDouble(ByVal i As Integer) As Double Implements IDataReader.GetDouble
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetDouble(i)
      End If
    End Function

    Public Function GetSmartDate(ByVal i As Integer, Optional ByVal MinIsEmpty As Boolean = True) As SmartDate
      If mDataReader.IsDBNull(i) Then
        Return New SmartDate(MinIsEmpty)

      Else
        Return New SmartDate(mDataReader.GetDateTime(i), MinIsEmpty)
      End If
    End Function

    Public Function GetGuid(ByVal i As Integer) As Guid Implements IDataReader.GetGuid
      If mDataReader.IsDBNull(i) Then
        Return Guid.Empty
      Else
        Return mDataReader.GetGuid(i)
      End If
    End Function

    Public Function Read() As Boolean Implements IDataReader.Read
      Return mDataReader.Read
    End Function

    Public Function NextResult() As Boolean Implements IDataReader.NextResult
      Return mDataReader.NextResult()
    End Function

    Public Sub Close() Implements IDataReader.Close
      mDataReader.Close()
    End Sub

    Public ReadOnly Property Depth() As Integer Implements System.Data.IDataReader.Depth
      Get
        Return mDataReader.Depth
      End Get
    End Property

    Public Sub Dispose() Implements System.IDisposable.Dispose
      mDataReader.Dispose()
    End Sub

    Public ReadOnly Property FieldCount() As Integer Implements System.Data.IDataRecord.FieldCount
      Get
        Return mDataReader.FieldCount
      End Get
    End Property

    Public Function GetBoolean(ByVal i As Integer) As Boolean Implements System.Data.IDataRecord.GetBoolean
      If mDataReader.IsDBNull(i) Then
        Return False
      Else
        Return mDataReader.GetBoolean(i)
      End If
    End Function

    Public Function GetByte(ByVal i As Integer) As Byte Implements System.Data.IDataRecord.GetByte
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetByte(i)
      End If
    End Function

    Public Function GetBytes(ByVal i As Integer, ByVal fieldOffset As Long, ByVal buffer() As Byte, ByVal bufferoffset As Integer, ByVal length As Integer) As Long Implements System.Data.IDataRecord.GetBytes
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length)
      End If
    End Function

    Public Function GetChar(ByVal i As Integer) As Char Implements System.Data.IDataRecord.GetChar
      If mDataReader.IsDBNull(i) Then
        Return Char.MinValue
      Else
        Return mDataReader.GetChar(i)
      End If
    End Function

    Public Function GetChars(ByVal i As Integer, ByVal fieldoffset As Long, ByVal buffer() As Char, ByVal bufferoffset As Integer, ByVal length As Integer) As Long Implements System.Data.IDataRecord.GetChars
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length)
      End If
    End Function

    Public Function GetData(ByVal i As Integer) As System.Data.IDataReader Implements System.Data.IDataRecord.GetData
      Return mDataReader.GetData(i)
    End Function

    Public Function GetDataTypeName(ByVal i As Integer) As String Implements System.Data.IDataRecord.GetDataTypeName
      Return mDataReader.GetDataTypeName(i)
    End Function

    Public Function GetDateTime(ByVal i As Integer) As Date Implements System.Data.IDataRecord.GetDateTime
      If mDataReader.IsDBNull(i) Then
        Return Date.MinValue
      Else
        Return mDataReader.GetDateTime(i)
      End If
    End Function

    Public Function GetDecimal(ByVal i As Integer) As Decimal Implements System.Data.IDataRecord.GetDecimal
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetDecimal(i)
      End If
    End Function

    Public Function GetFieldType(ByVal i As Integer) As System.Type Implements System.Data.IDataRecord.GetFieldType
      Return mDataReader.GetFieldType(i)
    End Function

    Public Function GetFloat(ByVal i As Integer) As Single Implements System.Data.IDataRecord.GetFloat
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetFloat(i)
      End If
    End Function

    Public Function GetInt16(ByVal i As Integer) As Short Implements System.Data.IDataRecord.GetInt16
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetInt16(i)
      End If
    End Function

    Public Function GetInt64(ByVal i As Integer) As Long Implements System.Data.IDataRecord.GetInt64
      If mDataReader.IsDBNull(i) Then
        Return 0
      Else
        Return mDataReader.GetInt64(i)
      End If
    End Function

    Public Function GetName(ByVal i As Integer) As String Implements System.Data.IDataRecord.GetName
      Return mDataReader.GetName(i)
    End Function

    Public Function GetOrdinal(ByVal name As String) As Integer Implements System.Data.IDataRecord.GetOrdinal
      Return mDataReader.GetOrdinal(name)
    End Function

    Public Function GetSchemaTable() As System.Data.DataTable Implements System.Data.IDataReader.GetSchemaTable
      Return mDataReader.GetSchemaTable
    End Function

    Public Function GetValues(ByVal values() As Object) As Integer Implements System.Data.IDataRecord.GetValues
      Return mDataReader.GetValues(values)
    End Function

    Public ReadOnly Property IsClosed() As Boolean Implements System.Data.IDataReader.IsClosed
      Get
        Return mDataReader.IsClosed
      End Get
    End Property

    Public Function IsDBNull(ByVal i As Integer) As Boolean Implements System.Data.IDataRecord.IsDBNull
      Return mDataReader.IsDBNull(i)
    End Function

    Default Public Overloads ReadOnly Property Item(ByVal i As Integer) As Object Implements System.Data.IDataRecord.Item
      Get
        If mDataReader.IsDBNull(i) Then
          Return Nothing
        Else
          Return mDataReader.Item(i)
        End If
      End Get
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal name As String) As Object Implements System.Data.IDataRecord.Item
      Get
        Dim value As Object = mDataReader.Item(name)
        If DBNull.Value.Equals(value) Then
          Return Nothing
        Else
          Return value
        End If
      End Get
    End Property

    Public ReadOnly Property RecordsAffected() As Integer Implements System.Data.IDataReader.RecordsAffected
      Get
        Return mDataReader.RecordsAffected
      End Get
    End Property
  End Class

End Namespace
