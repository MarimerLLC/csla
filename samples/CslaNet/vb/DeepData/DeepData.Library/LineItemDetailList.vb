<Serializable()> _
Public Class LineItemDetailList
  Inherits ReadOnlyListBase(Of LineItemDetailList, LineItemDetailInfo)

#Region " Factory Methods "

  Friend Shared Function NewList() As LineItemDetailList

    Return New LineItemDetailList

  End Function

  Private Sub New()

  End Sub

#End Region

#Region " Data Access "

  Friend Sub LoadChild(ByVal data As SafeDataReader)

    IsReadOnly = False
    Add(LineItemDetailInfo.GetItem(data))
    IsReadOnly = True

  End Sub

#End Region

End Class
