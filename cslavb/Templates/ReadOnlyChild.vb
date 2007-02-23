Imports System.Data.SqlClient

<Serializable()> _
Public Class ReadOnlyChild
  Inherits ReadOnlyBase(Of ReadOnlyChild)

#Region " Business Methods "

  Private mId As Integer
  Private mData As String = ""

  Public ReadOnly Property Id() As Integer
    Get
      Return mId
    End Get
  End Property

  Public ReadOnly Property Data() As String
    Get
      Return mData
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return mId
  End Function

  Public Overrides Function ToString() As String
    Return mData
  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function GetReadOnlyChild( _
    ByVal dr As SqlDataReader) As ReadOnlyChild

    Return New ReadOnlyChild(dr)
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Private Sub New(ByVal dr As SqlDataReader)
    Fetch(dr)
  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SqlDataReader)
    ' load values
    mId = dr.GetInt32(0)
    mData = dr.GetString(1)
  End Sub

#End Region

End Class
