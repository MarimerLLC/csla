Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

<Serializable()> _
  Public Class ReadOnlyChildList
  Inherits ReadOnlyListBase(Of ReadOnlyChildList, ReadOnlyChild)

#Region "Authorization Rules"

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    'AuthorizationRules.AllowGet(GetType(ReadOnlyChildList), "Role")
  End Sub

#End Region

#Region "Factory Methods"

  Friend Shared Function GetReadOnlyChildList(ByVal childData As Object) As ReadOnlyChildList
    Return DataPortal.FetchChild(Of ReadOnlyChildList)(childData)
  End Function

#End Region

#Region "Data Access"

  Private Sub Child_Fetch(ByVal childData As Object)
    RaiseListChangedEvents = False
    IsReadOnly = False
    ' TODO: load values 
    For Each child As Object In DirectCast(childData, List(Of Object))
      Add(ReadOnlyChild.GetReadOnlyChild(child))
    Next
    IsReadOnly = True
    RaiseListChangedEvents = True
  End Sub

#End Region
End Class
