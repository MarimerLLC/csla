Imports System.Data.SqlClient

<Serializable()> _
Public Class EditableChildList
  Inherits BusinessListBase(Of EditableChildList, EditableChild)

#Region " Factory Methods "

  Friend Shared Function NewEditableChildList() As EditableChildList
    Return New EditableChildList
  End Function

  Friend Shared Function GetEditableChildList(ByVal dr As SqlDataReader) As EditableChildList
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

    While dr.Read
      Add(EditableChild.GetEditableChild(dr))
    End While

  End Sub

  Friend Sub Update()

    For Each item As EditableChild In DeletedList
      item.DeleteSelf()
    Next
    DeletedList.Clear()

    For Each item As EditableChild In Me
      If item.IsNew Then
        item.Insert()

      Else
        item.Update()
      End If
    Next

  End Sub

#End Region

End Class
