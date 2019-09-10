Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

<Serializable()>
Public Class ReadOnlyList
  Inherits ReadOnlyListBase(Of ReadOnlyList, ReadOnlyChild)

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    ' AuthorizationRules.AllowGet(GetType(ReadOnlyList), "Role")
  End Sub

  <Fetch>
  Private Sub Fetch(ByVal id As String)
    RaiseListChangedEvents = False
    IsReadOnly = False
    ' TODO: load values 
    Dim objectData As Object = Nothing
    For Each child As Object In DirectCast(objectData, List(Of Object))
      Add(DataPortal.FetchChild(Of ReadOnlyChild)(child))
    Next
    IsReadOnly = True
    RaiseListChangedEvents = True
  End Sub

End Class
