Imports System.Data.SqlClient
Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

<Serializable()> _
Public MustInherit Class NameValueList
  Inherits NameObjectCollectionBase

  Implements ICloneable

#Region " Clone "

  ' all business objects _must_ be serializable
  ' and thus can be cloned - this just clinches
  ' the deal
  Public Function Clone() As Object Implements ICloneable.Clone
    Dim buffer As New MemoryStream
    Dim formatter As New BinaryFormatter

    formatter.Serialize(buffer, Me)
    buffer.Position = 0
    Return formatter.Deserialize(buffer)
  End Function

#End Region

#Region " Collection methods "

  Default Public ReadOnly Property Item(ByVal Index As Integer) As String
    Get
      Return CStr(MyBase.BaseGet(Index))
    End Get
  End Property

  Default Public ReadOnly Property Item(ByVal Name As String) As String
    Get
      Return CStr(MyBase.BaseGet(Name))
    End Get
  End Property

  Protected Sub Add(ByVal Name As String, ByVal Value As String)
    MyBase.BaseAdd(Name, Value)
  End Sub

  Public ReadOnly Property Key(ByVal Item As String) As String
    Get
      Dim keyName As String
      For Each keyName In Me
        If Me.Item(keyName) = Item Then
          Return keyName
        End If
      Next
      ' we didn't find a match - throw an exception
      Throw New ApplicationException("No matching item in collection")
    End Get
  End Property

#End Region

#Region " Create and Load "

  Protected Sub New()
    ' prevent public creation
  End Sub

  Protected Sub New(ByVal info As SerializationInfo, _
      ByVal context As StreamingContext)
    MyBase.New(info, context)
  End Sub

#End Region

#Region " Data Access "

  Private Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - create not allowed")
  End Sub

  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - fetch not allowed")
  End Sub

  Private Sub DataPortal_Update()
    Throw New NotSupportedException("Invalid operation - update not allowed")
  End Sub

  Private Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - delete not allowed")
  End Sub

  Protected Function DB(ByVal DatabaseName As String) As String
    Return ConfigurationSettings.AppSettings("DB:" & DatabaseName)
  End Function

  ' this provides default/simple loading for most lists
  ' called to load data from the database
  Protected Sub SimpleFetch(ByVal DataBaseName As String, ByVal TableName As String, ByVal NameColumn As String, ByVal ValueColumn As String)

    Dim cn As New SqlConnection(DB(DataBaseName))
    Dim cm As New SqlCommand

    cn.Open()
    Try
      With cm
        .Connection = cn
        .CommandText = _
          "SELECT " & NameColumn & "," & ValueColumn & " FROM " & TableName
        Dim dr As New Data.SafeDataReader(.ExecuteReader)
        Try
          While dr.Read()
            Add(CStr(dr.GetValue(0)), CStr(dr.GetValue(1)))
          End While

        Finally
          dr.Close()
        End Try

      End With

    Finally
      cn.Close()
    End Try

  End Sub

#End Region

End Class
