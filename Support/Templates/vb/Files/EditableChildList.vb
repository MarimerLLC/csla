Imports Csla.Security

<Serializable()> _
Public Class EditableChildList
  Inherits BusinessListBase(Of EditableChildList, EditableChild)

#Region " Factory Methods "

  Friend Shared Function NewEditableChildList() As EditableChildList
    Return DataPortal.CreateChild(Of EditableChildList)()
  End Function

  Friend Shared Function GetEditableChildList(ByVal childData As Object) As EditableChildList
    Return DataPortal.FetchChild(Of EditableChildList)(childData)
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
