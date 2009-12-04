Imports System.Data.SqlClient

<Serializable()> _
Public Class ConsultantNVList
  Inherits NameValueListBase(Of Integer, String)

#Region " Factory Methods "

  Private Shared _list As ConsultantNVList

  Public Shared Function GetList() As ConsultantNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ConsultantNVList)(New Criteria(False))
    End If
    Return _list

  End Function

  Public Shared Function GetBenchList() As ConsultantNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ConsultantNVList)(New Criteria(True))
    End If
    Return _list

  End Function

  Public Shared Sub FlushCache()

    _list = Nothing

  End Sub

  Private Sub New()

    ' require use of factory methods

  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Shadows Class Criteria

    Private _benchOnly As Boolean
    Public ReadOnly Property BenchOnly() As Boolean
      Get
        Return _benchOnly
      End Get
    End Property

    Public Sub New(ByVal benchOnly As Boolean)
      _benchOnly = benchOnly
    End Sub

  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Dim svc As New BenchService.BenchServiceClient
    Dim list As New List(Of BenchService.ConsultantData)(svc.GetConsultantList(criteria.BenchOnly))
    IsReadOnly = False
    For Each item As BenchService.ConsultantData In list
      Add(New NameValuePair(item.Id, item.Name))
    Next
    IsReadOnly = True

  End Sub

#End Region

End Class
