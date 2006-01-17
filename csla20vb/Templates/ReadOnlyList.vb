Imports System.Data.SqlClient

<Serializable()> _
Public Class ReadOnlyList
  Inherits ReadOnlyListBase(Of ReadOnlyList, ROLChild)

#Region " ROLChild "

  <Serializable()> _
  Public Class ROLChild
    Inherits ReadOnlyBase(Of ROLChild)

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

    Friend Shared Function GetChild(ByVal dr As SqlDataReader) As ROLChild
      Return New ROLChild(dr)
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

#End Region

#Region " Factory Methods "

  Public Shared Function GetList(ByVal filter As String) As ReadOnlyList
    Return DataPortal.Fetch(Of ReadOnlyList)(New Criteria(filter))
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria

    Private mFilter As String
    Public ReadOnly Property Filter() As String
      Get
        Return mFilter
      End Get
    End Property

    Public Sub New(ByVal filter As String)
      mFilter = filter
    End Sub
  End Class

  Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

    ' load values
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(ROLChild.GetChild(dr))
      End While
    End Using

  End Sub

#End Region

End Class
