Imports System.Data.SqlClient

<Serializable()> _
Public Class ReadOnlyList
  Inherits ReadOnlyListBase(Of ReadOnlyList, ReadOnlyChild)

#Region " Authorization Rules "

  Public Shared Function CanGetObject() As Boolean
    ' TODO: customize to check user role
    'Return ApplicationContext.User.IsInRole("")
    Return True
  End Function

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

    RaiseListChangedEvents = False
    IsReadOnly = False
    ' load values
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(ReadOnlyChild.GetReadOnlyChild(dr))
      End While
    End Using
    IsReadOnly = True
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
