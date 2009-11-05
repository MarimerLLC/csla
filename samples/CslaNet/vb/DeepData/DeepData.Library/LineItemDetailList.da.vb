Option Strict Off

Partial Public Class LineItemDetailList

  Friend Sub LoadDetail(ByVal data)

    IsReadOnly = False
    Dim detail
    For Each detail In data
      Add(LineItemDetailInfo.GetItem(detail))
    Next
    IsReadOnly = True

  End Sub

End Class
