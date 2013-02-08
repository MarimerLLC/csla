Imports Csla.Security

<Serializable()> _
Public Class EditableChildBindingList
  Inherits BusinessBindingListBase(Of EditableChildBindingList, EditableChild)

#Region " Factory Methods "

  Friend Shared Function NewEditableChildBindingList() As EditableChildBindingList
    Return DataPortal.CreateChild(Of EditableChildBindingList)()
  End Function

  Friend Shared Function GetEditableChildBindingList(ByVal childData As Object) As EditableChildBindingList
    Return DataPortal.FetchChild(Of EditableChildBindingList)(childData)
  End Function

  Private Sub New()
    'use factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Sub Child_Fetch(ByVal childData As Object)
    RaiseListChangedEvents = False
    For Each child As Object In CType(childData, IList(Of Object))
      Me.Add(EditableChild.GetEditableChild(child))
    Next
    RaiseListChangedEvents = True
  End Sub

#End Region

End Class
