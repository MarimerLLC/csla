Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

<Serializable()>
Public Class EditableRootBindingList
  Inherits BusinessListBase(Of EditableRootBindingList, EditableChild)

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    'Csla.Rules.BusinessRules.AddRule(GetType(EditableRootBindingList), New Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.GetObject, "Role"))
  End Sub

  <Fetch>
  Private Sub Fetch(ByVal criteria As Integer)
    RaiseListChangedEvents = False
    ' TODO: load values into memory 
    Dim childData As Object = Nothing
    For Each item As Object In DirectCast(childData, List(Of Object))
      Me.Add(DataPortal.FetchChild(Of EditableChild)(childData))
    Next
    RaiseListChangedEvents = True
  End Sub

  <Update>
  Private Sub Update()
    ' open database and update child objects
    'MyBase.Child_Update()
  End Sub

End Class
