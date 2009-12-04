Partial Public Class ClientList

  Private Sub ClientList_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    Using New StatusBusy("Loading data...")
      Dim list As Library.ClientList = Library.ClientList.GetList

      list.BeginEdit()

      Me.DataContext = list

      ApplyAuthorization()
    End Using

  End Sub

  Private Sub ApplyAuthorization()

  End Sub

  Private Sub RemoveItem(ByVal sender As Object, ByVal e As EventArgs)

    Dim btn As Button = CType(sender, Button)
    Dim id As Integer = CInt(btn.Tag)
    Dim list As Library.ClientList = CType(Me.DataContext, Library.ClientList)
    For Each item As Library.ClientEdit In list
      If item.Id = id Then
        list.Remove(item)
        Exit For
      End If
    Next

  End Sub

  Private Sub AddItemButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles AddItemButton.Click

    CType(Me.DataContext, Library.ClientList).AddNew()

  End Sub

  Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles CancelButton.Click

    Dim list As Library.ClientList = CType(Me.DataContext, Library.ClientList)
    list.CancelEdit()
    list.BeginEdit()

  End Sub

  Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveButton.Click

    Dim list As Library.ClientList = CType(Me.DataContext, Library.ClientList)
    list = SaveObject(list)
    Me.DataContext = Nothing
    Me.DataContext = list

  End Sub

End Class
