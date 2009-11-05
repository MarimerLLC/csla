Public Class Form1

  Private Sub OrderBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    Me.Validate()

  End Sub

  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Me.OrderListBindingSource.DataSource = OrderList.GetList

  End Sub

End Class
