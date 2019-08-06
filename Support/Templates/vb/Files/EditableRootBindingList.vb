Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class EditableRootBindingList
  Inherits BusinessListBase(Of EditableRootBindingList, EditableChild)

#Region "Business Rules"

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    'Csla.Rules.BusinessRules.AddRule(GetType(EditableRootBindingList), New Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.GetObject, "Role"))
  End Sub

#End Region

#Region "Factory Methods"

  Public Shared Function NewEditableRootBindingList() As EditableRootBindingList
    Return DataPortal.Create(Of EditableRootBindingList)()
  End Function

  Public Shared Function GetEditableRootBindingList(ByVal id As Integer) As EditableRootBindingList
    Return DataPortal.Fetch(Of EditableRootBindingList)(id)
  End Function

#End Region

#Region "Data Access"

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Integer)
    RaiseListChangedEvents = False
    ' TODO: load values into memory 
    Dim childData As Object = Nothing
    For Each item As Object In DirectCast(childData, List(Of Object))
      Me.Add(EditableChild.GetEditableChild(childData))
    Next
    RaiseListChangedEvents = True
  End Sub

  Protected Overrides Sub DataPortal_Update()
    ' open database and update child objects
    'MyBase.Child_Update()
  End Sub

#End Region
End Class
