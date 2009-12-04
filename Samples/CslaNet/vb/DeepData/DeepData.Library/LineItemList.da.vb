Option Strict Off

Partial Public Class LineItemList

  Friend Sub LoadItems(ByVal data)

    IsReadOnly = False
    Dim line
    For Each line In data
      Add(LineItemInfo.GetItem(line))
    Next
    IsReadOnly = True

  End Sub

End Class
