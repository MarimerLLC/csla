Imports System.Data.SqlClient

<Serializable()> _
Public Class ClientProjectNVList
  Inherits NameValueListBase(Of Integer, String)

  Private Shared _list As ClientProjectNVList

  Public Shared Function GetList() As ClientProjectNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ClientProjectNVList)()
    End If
    Return _list

  End Function

  Public Shared Sub FlushCache()

    _list = Nothing

  End Sub

  Private Sub New()

    ' require use of factory methods

  End Sub

  Private Overloads Sub DataPortal_Fetch()

    Dim svc As New SalesService.SalesServiceClient
    Dim projectList() As SalesService.ProjectSummaryData = svc.GetFullProjectList
    ' load business objects from DTOs
    IsReadOnly = False
    For Each item As SalesService.ProjectSummaryData In projectList
      Add(New NameValuePair(item.Id, item.Name))
    Next
    IsReadOnly = True

  End Sub

End Class
