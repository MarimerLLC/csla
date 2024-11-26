﻿Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

<Serializable()>
Public Class ReadOnlyChildList
  Inherits ReadOnlyListBase(Of ReadOnlyChildList, ReadOnlyChild)

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    'AuthorizationRules.AllowGet(GetType(ReadOnlyChildList), "Role")
  End Sub

  <FetchChild>
  Private Sub Fetch(ByVal childData As Object)
    RaiseListChangedEvents = False
    IsReadOnly = False
    ' TODO: load values 
    Dim dataPortal As IChildDataPortal(Of ReadOnlyChild) = ApplicationContext.GetRequiredService(Of IChildDataPortal(Of ReadOnlyChild))()
    For Each child As Object In DirectCast(childData, List(Of Object))
      Add(dataPortal.FetchChild(child))
    Next
    IsReadOnly = True
    RaiseListChangedEvents = True
  End Sub

End Class
