<Serializable()> _
Public Class LineItemList
  Inherits ReadOnlyListBase(Of LineItemList, LineItemInfo)

#Region " Business Methods "

  Public Function FindById(ByVal id As Integer) As LineItemInfo

    For Each child As LineItemInfo In Me
      If child.Id = id Then
        Return child
      End If
    Next
    Return Nothing

  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function NewList() As LineItemList

    Return New LineItemList

  End Function

  Private Sub New()

  End Sub

#End Region

#Region " Data Access "

  Friend Sub LoadChild(ByVal data As SafeDataReader)

    IsReadOnly = False
    Add(LineItemInfo.GetItem(data))
    IsReadOnly = True

  End Sub

#End Region

End Class
