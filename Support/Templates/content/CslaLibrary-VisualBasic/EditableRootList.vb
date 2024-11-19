Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

<Serializable()>
Public Class EditableRootList
  Inherits BusinessListBase(Of EditableRootList, EditableChild)

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    'Csla.Rules.BusinessRules.AddRule(GetType(EditableRootList), New Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.GetObject, "Role"))
  End Sub

  <Fetch>
  Private Sub Fetch(ByVal criteria As Integer)
    RaiseListChangedEvents = False
    ' TODO: load values into memory 
    Dim childData As Object = Nothing
    Dim dataPortal As IChildDataPortal(Of EditableChild) = ApplicationContext.GetRequiredService(Of IChildDataPortal(Of EditableChild))()
    For Each item As Object In DirectCast(childData, List(Of Object))
      Me.Add(dataPortal.FetchChild(childData))
    Next
    RaiseListChangedEvents = True
  End Sub

  <Update>
  Private Sub Update()
    ' open database and update child objects
    'MyBase.Child_Update()
  End Sub

End Class
