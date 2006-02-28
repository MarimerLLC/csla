Imports System.Data.SqlClient

<Serializable()> _
Public Class EditableChildList
  Inherits BusinessListBase(Of EditableChildList, EditableChild)

#Region " Factory Methods "

  Friend Shared Function NewEditableChildList() As EditableChildList
    Return New EditableChildList
  End Function

  Friend Shared Function GetEditableChildList( _
    ByVal dr As SqlDataReader) As EditableChildList

    Return New EditableChildList(dr)
  End Function

  Private Sub New()
    MarkAsChild()
  End Sub

  Private Sub New(ByVal dr As SqlDataReader)
    MarkAsChild()
    Fetch(dr)
  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SqlDataReader)

    RaiseListChangedEvents = False
    While dr.Read
      Add(EditableChild.GetEditableChild(dr))
    End While
    RaiseListChangedEvents = True

  End Sub

  Friend Sub Update(ByVal parent As Object)

    RaiseListChangedEvents = False
    For Each item As EditableChild In DeletedList
      item.DeleteSelf()
    Next
    DeletedList.Clear()

    For Each item As EditableChild In Me
      If item.IsNew Then
        item.Insert(parent)

      Else
        item.Update(parent)
      End If
    Next
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
