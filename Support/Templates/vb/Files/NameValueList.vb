Imports System
Imports Csla

<Serializable()>
Public Class NameValueList
  Inherits NameValueListBase(Of Integer, String)

  Private Shared _list As NameValueList

  Public Shared Function GetNameValueList() As NameValueList
    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of NameValueList)()
    End If
    Return _list
  End Function

  Public Shared Sub InvalidateCache()
    _list = Nothing
  End Sub

  <Fetch>
  Private Sub Fetch()
    RaiseListChangedEvents = False
    IsReadOnly = False
    ' TODO: load values 
    Dim listData As Object = Nothing
    For Each oneItem As Object In CType(listData, IList(Of Object))
      Add(New NameValueList.NameValuePair(0, ""))
    Next
    IsReadOnly = True
    RaiseListChangedEvents = True
  End Sub

End Class
